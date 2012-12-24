﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.Bass.AddOn.Mix;

namespace FDK
{
	public class CSoundDeviceASIO : ISoundDevice
	{
		// プロパティ

		public ESoundDeviceType e出力デバイス
		{
			get;
			protected set;
		}
		public long n実出力遅延ms
		{
			get;
			protected set;
		}

		// CSoundTimer 用に公開しているプロパティ

		public long n経過時間ms
		{
			get;
			protected set;
		}
		public long n経過時間を更新したシステム時刻ms
		{
			get;
			protected set;
		}
		public CTimer tmシステムタイマ
		{
			get;
			protected set;
		}


		// メソッド

		public CSoundDeviceASIO( long nバッファサイズms )
		{
			// 初期化。

			this.e出力デバイス = ESoundDeviceType.Unknown;
			this.n実出力遅延ms = 0;
			this.n経過時間ms = 0;
			this.n経過時間を更新したシステム時刻ms = CTimer.n未使用;
			this.tmシステムタイマ = new CTimer( CTimer.E種別.MultiMedia );


			// BASS.NET ユーザ登録（BASSスプラッシュが非表示になる）。

			BassNet.Registration( "dtx0266@gmail.com", "2X9182617152222" );


			// BASS のバージョンチェック。

			int nBASSVersion = Utils.HighWord( Bass.BASS_GetVersion() );
			if( nBASSVersion != Bass.BASSVERSION )
				throw new DllNotFoundException( string.Format( "bass.dll のバージョンが異なります({0})。このプログラムはバージョン{1}で動作します。", nBASSVersion, Bass.BASSVERSION ) );

			int nBASSMixVersion = Utils.HighWord( BassMix.BASS_Mixer_GetVersion() );
			if( nBASSMixVersion != BassMix.BASSMIXVERSION )
				throw new DllNotFoundException( string.Format( "bassmix.dll のバージョンが異なります({0})。このプログラムはバージョン{1}で動作します。", nBASSMixVersion, BassMix.BASSMIXVERSION ) );

			int nBASSASIO = Utils.HighWord( BassAsio.BASS_ASIO_GetVersion() );
			if( nBASSASIO != BassAsio.BASSASIOVERSION )
				throw new DllNotFoundException( string.Format( "bassasio.dll のバージョンが異なります({0})。このプログラムはバージョン{1}で動作します。", nBASSASIO, BassAsio.BASSASIOVERSION ) );


			// BASS の設定。

			Debug.Assert( Bass.BASS_SetConfig( BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0 ),		// 0:BASSストリームの自動更新を行わない。（BASSWASAPIから行うため）
				string.Format( "BASS_SetConfig() に失敗しました。[{0}", Bass.BASS_ErrorGetCode() ) );


			// BASS の初期化。

			int nデバイス = 0;		// 0:"no device" … BASS からはデバイスへアクセスさせない。アクセスは BASSASIO アドオンから行う。
			int n周波数 = 44100;	// 仮決め。最終的な周波数はデバイス（≠ドライバ）が決める。
			if( !Bass.BASS_Init( nデバイス, n周波数, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero ) )
				throw new Exception( string.Format( "BASS の初期化に失敗しました。[{0}]", Bass.BASS_ErrorGetCode().ToString() ) );

			#region [ デバッグ用: ASIOデバイスのenumerateと、ログ出力 ]
			//int a, count = 0;
			//BASS_ASIO_DEVICEINFO asioDevInfo;
			//for ( a = 0; ( asioDevInfo = BassAsio.BASS_ASIO_GetDeviceInfo( a ) ) != null; a++ )
			//{
			//    Trace.TraceInformation( "ASIO Device {0}: {1}", a, asioDevInfo.name );
			//    count++; // count it
			//}
			#endregion

			// BASS ASIO の初期化。

			nデバイス = 0;			// デフォルトデバイス
			if( BassAsio.BASS_ASIO_Init( nデバイス, BASSASIOInit.BASS_ASIO_THREAD ) )	// 専用スレッドにて起動
			{
				#region [ ASIO の初期化に成功。]
				//-----------------
				this.e出力デバイス = ESoundDeviceType.ASIO;
				var asioInfo = BassAsio.BASS_ASIO_GetInfo();
				this.n出力チャンネル数 = asioInfo.outputs;
				this.db周波数 = BassAsio.BASS_ASIO_GetRate();
				this.fmtASIOデバイスフォーマット = BassAsio.BASS_ASIO_ChannelGetFormat( false, 0 );

				Trace.TraceInformation( "BASS を初期化しました。(ASIO, デバイス:\"{0}\", 入力{1}, 出力{2}, {3}Hz, バッファ{4}～{6}sample ({5:0.###}～{7:0.###}ms), フォーマット:{8})",
					asioInfo.name,
					asioInfo.inputs,
					asioInfo.outputs,
					this.db周波数.ToString( "0.###" ),
					asioInfo.bufmin, asioInfo.bufmin * 1000 / this.db周波数,
					asioInfo.bufmax, asioInfo.bufmax * 1000 / this.db周波数,
					this.fmtASIOデバイスフォーマット.ToString()
					);
				//-----------------
				#endregion
			}
			else
			{
				#region [ ASIO の初期化に失敗。]
				//-----------------
				Bass.BASS_Free();
				throw new Exception( string.Format( "BASS (ASIO) の初期化に失敗しました。[{0}]", BassAsio.BASS_ASIO_ErrorGetCode().ToString() ) );
				//-----------------
				#endregion
			}


			// ASIO 出力チャンネルの初期化。

			this.tAsioProc = new ASIOPROC( this.tAsio処理 );		// アンマネージに渡す delegate は、フィールドとして保持しておかないとGCでアドレスが変わってしまう。
			if ( !BassAsio.BASS_ASIO_ChannelEnable( false, 0, this.tAsioProc, IntPtr.Zero ) )		// 出力チャンネル0 の有効化。
			{
				#region [ ASIO 出力チャンネルの初期化に失敗。]
				//-----------------
				Bass.BASS_Free();
				throw new Exception( string.Format( "Failed BASS_ASIO_ChannelEnable() [{0}]", BassAsio.BASS_ASIO_ErrorGetCode().ToString() ) );
				//-----------------
				#endregion
			}
			//for( int i = 1; i < this.n出力チャンネル数; i++ )
			//	BassAsio.BASS_ASIO_ChannelJoin( false, i, 0 );
			if (!BassAsio.BASS_ASIO_ChannelJoin( false, 1, 0 ))										// 出力チャンネル1をチャンネル0 とグループ化。（ステレオ限定）
			{
				#region [ 初期化に失敗。]
				//-----------------
				Bass.BASS_Free();
				throw new Exception( string.Format( "Failed BASS_ASIO_ChannelJoin() [{0}]", BassAsio.BASS_ASIO_ErrorGetCode().ToString() ) );
				//-----------------
				#endregion
			}
			if ( !BassAsio.BASS_ASIO_ChannelSetFormat( false, 0, this.fmtASIOチャンネルフォーマット ) )	// 出力チャンネル0のフォーマット
			{
				#region [ ASIO 出力チャンネルの初期化に失敗。]
				//-----------------
				Bass.BASS_Free();
				throw new Exception( string.Format( "Failed BASS_ASIO_ChannelSetFormat() [{0}]", BassAsio.BASS_ASIO_ErrorGetCode().ToString() ) );
				//-----------------
				#endregion
			}


			// ASIO 出力と同じフォーマットを持つ BASS ミキサーを作成。

			var flag = BASSFlag.BASS_MIXER_NONSTOP | BASSFlag.BASS_STREAM_DECODE;	// デコードのみ＝発声しない。ASIO に出力されるだけ。
			if( this.fmtASIOデバイスフォーマット == BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT )
				flag |= BASSFlag.BASS_SAMPLE_FLOAT;
			this.hMixer = BassMix.BASS_Mixer_StreamCreate( (int) this.db周波数, this.n出力チャンネル数, flag );
			if( this.hMixer == 0 )
				throw new Exception( string.Format( "BASSミキサの作成に失敗しました。[{0}]", Bass.BASS_ErrorGetCode() ) );


			// BASS ミキサーの1秒あたりのバイト数を算出。

			var mixerInfo = Bass.BASS_ChannelGetInfo( this.hMixer );
			int nサンプルサイズbyte = 0;
			switch( this.fmtASIOチャンネルフォーマット )
			{
				case BASSASIOFormat.BASS_ASIO_FORMAT_16BIT: nサンプルサイズbyte = 2; break;
				case BASSASIOFormat.BASS_ASIO_FORMAT_24BIT: nサンプルサイズbyte = 3; break;
				case BASSASIOFormat.BASS_ASIO_FORMAT_32BIT: nサンプルサイズbyte = 4; break;
				case BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT: nサンプルサイズbyte = 4; break;
			}
			long nミキサーの1サンプルあたりのバイト数 = /*mixerInfo.chans*/ 2 * nサンプルサイズbyte;
			this.nミキサーの1秒あたりのバイト数 = nミキサーの1サンプルあたりのバイト数 * mixerInfo.freq;


			// 出力を開始。

			this.nバッファサイズsample = (int) ( nバッファサイズms * this.db周波数 / 1000.0 );
			if ( !BassAsio.BASS_ASIO_Start( this.nバッファサイズsample ) )		// 範囲外の値を指定した場合は自動的にデフォルト値に設定される。
			{
				Bass.BASS_Free();
				throw new Exception( "ASIO デバイス出力開始に失敗しました。" + BassAsio.BASS_ASIO_ErrorGetCode().ToString() );
			}
			else
			{
				int n遅延sample = BassAsio.BASS_ASIO_GetLatency( false );	// この関数は BASS_ASIO_Start() 後にしか呼び出せない。
				this.n実出力遅延ms = (long) ( n遅延sample * 1000.0f / this.db周波数 );
				Trace.TraceInformation( "ASIO デバイス出力開始：バッファ{0}sample [{1}ms(希望{2}ms)]", n遅延sample, this.n実出力遅延ms, nバッファサイズms );
			}
		}

		public CSound tサウンドを作成する( string strファイル名 )
		{
			var sound = new CSound();
			sound.tASIOサウンドを作成する( strファイル名, this.hMixer );
			return sound;
		}
		public CSound tサウンドを作成する( byte[] byArrWAVファイルイメージ )
		{
			var sound = new CSound();
			sound.tASIOサウンドを作成する( byArrWAVファイルイメージ, this.hMixer );
			return sound;
		}
		public void tサウンドを作成する( string strファイル名, ref CSound sound )
		{
			sound.tASIOサウンドを作成する( strファイル名, this.hMixer );
		}
		public void tサウンドを作成する( byte[] byArrWAVファイルイメージ, ref CSound sound )
		{
			sound.tASIOサウンドを作成する( byArrWAVファイルイメージ, this.hMixer );
		}

		#region [ Dispose-Finallizeパターン実装 ]
		//-----------------
		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}
		protected void Dispose( bool bManagedDispose )
		{
			Bass.BASS_StreamFree( this.hMixer );
			BassAsio.BASS_ASIO_Free();	// システムタイマより先に呼び出すこと。（tAsio処理() の中でシステムタイマを参照してるため）
			Bass.BASS_Free();

			if( bManagedDispose )
			{
				C共通.tDisposeする( this.tmシステムタイマ );
				this.tmシステムタイマ = null;
			}
		}
		~CSoundDeviceASIO()
		{
			this.Dispose( false );
		}
		//-----------------
		#endregion

		protected int hMixer = -1;
		protected int n出力チャンネル数 = 0;
		protected double db周波数 = 0.0;
		protected int nバッファサイズsample = 0;
		protected BASSASIOFormat fmtASIOデバイスフォーマット = BASSASIOFormat.BASS_ASIO_FORMAT_UNKNOWN;
		protected BASSASIOFormat fmtASIOチャンネルフォーマット = BASSASIOFormat.BASS_ASIO_FORMAT_16BIT;		// 16bit 固定
		protected ASIOPROC tAsioProc = null;

		protected int tAsio処理( bool input, int channel, IntPtr buffer, int length, IntPtr user )
		{
			if( input ) return 0;


			// BASSミキサからの出力データをそのまま ASIO buffer へ丸投げ。

			int num = Bass.BASS_ChannelGetData( this.hMixer, buffer, length );		// num = 実際に転送した長さ
			if( num == -1 ) num = 0;


			// 経過時間を更新。
			// データの転送差分ではなく累積転送バイト数から算出する。

			this.n経過時間ms = ( this.n累積転送バイト数 * 1000 / this.nミキサーの1秒あたりのバイト数 ) - this.n実出力遅延ms;
			this.n経過時間を更新したシステム時刻ms = this.tmシステムタイマ.nシステム時刻ms;


			// 経過時間を更新後に、今回分の累積転送バイト数を反映。

			this.n累積転送バイト数 += num;
			return num;
		}

		private long nミキサーの1秒あたりのバイト数 = 0;
		private long n累積転送バイト数 = 0;
	}
}
