﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;
using System.Diagnostics;

namespace DTXMania
{
	internal class CAct演奏AVI : CActivity
	{
		// コンストラクタ

		public CAct演奏AVI()
		{
			base.b活性化してない = true;
		}
		/// <summary>
		/// プレビュームービーかどうか
		/// </summary>
		/// <remarks>
		/// On活性化()の前にフラグ操作すること。(活性化中に、本フラグを見て動作を変える部分があるため)
		/// </remarks>
		public bool bIsPreviewMovie
		{
			get;
			set;
		}
		public bool bHasBGA
		{
			get;
			set;
		}
		public bool bFullScreenMovie
		{
			get;
			set;
		}


		// メソッド

		public void Start( int nチャンネル番号, CDTX.CAVI rAVI, int n開始サイズW, int n開始サイズH, int n終了サイズW, int n終了サイズH, int n画像側開始位置X, int n画像側開始位置Y, int n画像側終了位置X, int n画像側終了位置Y, int n表示側開始位置X, int n表示側開始位置Y, int n表示側終了位置X, int n表示側終了位置Y, int n総移動時間ms, int n移動開始時刻ms )
		{
			if( nチャンネル番号 == (int) Ech定義.Movie || nチャンネル番号 == (int) Ech定義.MovieFull )
			{
				this.rAVI = rAVI;
				this.n開始サイズW = n開始サイズW;
				this.n開始サイズH = n開始サイズH;
				this.n終了サイズW = n終了サイズW;
				this.n終了サイズH = n終了サイズH;
				this.n画像側開始位置X = n画像側開始位置X;
				this.n画像側開始位置Y = n画像側開始位置Y;
				this.n画像側終了位置X = n画像側終了位置X;
				this.n画像側終了位置Y = n画像側終了位置Y;
				this.n表示側開始位置X = n表示側開始位置X;
				this.n表示側開始位置Y = n表示側開始位置Y;
				this.n表示側終了位置X = n表示側終了位置X;
				this.n表示側終了位置Y = n表示側終了位置Y;
				this.n総移動時間ms = n総移動時間ms;
				this.n移動開始時刻ms = ( n移動開始時刻ms != -1 ) ? n移動開始時刻ms : CSound管理.rc演奏用タイマ.n現在時刻;
				this.n前回表示したフレーム番号 = -1;
			}
		}
		public void SkipStart( int n移動開始時刻ms )
		{
			foreach( CDTX.CChip chip in CDTXMania.DTX.listChip )
			{
				if( chip.n発声時刻ms > n移動開始時刻ms )
				{
					break;
				}
				switch( chip.eAVI種別 )
				{
					case EAVI種別.AVI:
						{
							if( chip.rAVI != null )
							{
								// this.Start( chip.nチャンネル番号, chip.rAVI, 278, 355, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, chip.n発声時刻ms );
								this.Start( chip.nチャンネル番号, chip.rAVI, SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, chip.n発声時刻ms );
							}
							continue;
						}
					case EAVI種別.AVIPAN:
						{
							if( chip.rAVIPan != null )
							{
								this.Start( chip.nチャンネル番号, chip.rAVI, chip.rAVIPan.sz開始サイズ.Width, chip.rAVIPan.sz開始サイズ.Height, chip.rAVIPan.sz終了サイズ.Width, chip.rAVIPan.sz終了サイズ.Height, chip.rAVIPan.pt動画側開始位置.X, chip.rAVIPan.pt動画側開始位置.Y, chip.rAVIPan.pt動画側終了位置.X, chip.rAVIPan.pt動画側終了位置.Y, chip.rAVIPan.pt表示側開始位置.X, chip.rAVIPan.pt表示側開始位置.Y, chip.rAVIPan.pt表示側終了位置.X, chip.rAVIPan.pt表示側終了位置.Y, chip.n総移動時間, chip.n発声時刻ms );
							}
							continue;
						}
				}
			}
		}
		public void Stop()
		{
			if( ( this.rAVI != null ) && ( this.rAVI.avi != null ) )
			{
				this.n移動開始時刻ms = -1;
			}
		}
		public void Cont( int n再開時刻ms )
		{
			if( ( this.rAVI != null ) && ( this.rAVI.avi != null ) )
			{
				this.n移動開始時刻ms = n再開時刻ms;
			}
		}
		public unsafe int t進行描画( int x, int y )
		{
			if( !base.b活性化してない )
			{
				Rectangle rectangle;
				Rectangle rectangle2;
				if ( ( ( this.n移動開始時刻ms == -1 ) || ( this.rAVI == null ) ) || ( this.rAVI.avi == null ) )
				{
					return 0;
				}
				if ( this.tx描画用 == null )
				{
					return 0;
				}
				int time = (int) ( ( CSound管理.rc演奏用タイマ.n現在時刻 - this.n移動開始時刻ms ) * ( ( (double) CDTXMania.ConfigIni.n演奏速度 ) / 20.0 ) );
				int frameNoFromTime = this.rAVI.avi.GetFrameNoFromTime( time );
				if( ( this.n総移動時間ms != 0 ) && ( this.n総移動時間ms < time ) )
				{
					this.n総移動時間ms = 0;
					this.n移動開始時刻ms = -1;
					return 0;
				}
				if( ( this.n総移動時間ms == 0 ) && ( frameNoFromTime >= this.rAVI.avi.GetMaxFrameCount() ) )
				{
					if ( !bIsPreviewMovie )
					{
						this.n移動開始時刻ms = -1;
						return 0;
					}
					// PREVIEW時はループ再生する。移動開始時刻msを現時刻にして(=AVIを最初に巻き戻して)、ここまでに行った計算をやり直す。
					this.n移動開始時刻ms = CSound管理.rc演奏用タイマ.n現在時刻;
					time = (int) ( ( CSound管理.rc演奏用タイマ.n現在時刻 - this.n移動開始時刻ms ) * ( ( (double) CDTXMania.ConfigIni.n演奏速度 ) / 20.0 ) );
					frameNoFromTime = this.rAVI.avi.GetFrameNoFromTime( time );
				}
				if( ( this.n前回表示したフレーム番号 != frameNoFromTime ) && !this.bフレームを作成した )
				{
					this.pBmp = this.rAVI.avi.GetFramePtr( frameNoFromTime );
					this.n前回表示したフレーム番号 = frameNoFromTime;
					this.bフレームを作成した = true;
				}
				Size size = new Size( (int) this.rAVI.avi.nフレーム幅, (int) this.rAVI.avi.nフレーム高さ );
				// Size size2 = new Size( 278, 355);
				Size size2 = new Size( SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height);
				Size 開始サイズ = new Size( this.n開始サイズW, this.n開始サイズH );
				Size 終了サイズ = new Size( this.n終了サイズW, this.n終了サイズH );
				Point 画像側開始位置 = new Point( this.n画像側開始位置X, this.n画像側終了位置Y );
				Point 画像側終了位置 = new Point( this.n画像側終了位置X, this.n画像側終了位置Y );
				Point 表示側開始位置 = new Point( this.n表示側開始位置X, this.n表示側開始位置Y );
				Point 表示側終了位置 = new Point( this.n表示側終了位置X, this.n表示側終了位置Y );
				long num3 = this.n総移動時間ms;
				long num4 = this.n移動開始時刻ms;
				if( CSound管理.rc演奏用タイマ.n現在時刻 < num4 )
				{
					num4 = CSound管理.rc演奏用タイマ.n現在時刻;
				}
				time = (int) ( ( CSound管理.rc演奏用タイマ.n現在時刻 - num4 ) * ( ( (double) CDTXMania.ConfigIni.n演奏速度 ) / 20.0 ) );
				if( num3 == 0 )
				{
					rectangle = new Rectangle( 画像側開始位置, 開始サイズ );
					rectangle2 = new Rectangle( 表示側開始位置, 開始サイズ );
				}
				else
				{
					double num5 = ( (double) time ) / ( (double) num3 );
					Size size5 = new Size( 開始サイズ.Width + ( (int) ( ( 終了サイズ.Width - 開始サイズ.Width ) * num5 ) ), 開始サイズ.Height + ( (int) ( ( 終了サイズ.Height - 開始サイズ.Height ) * num5 ) ) );
					rectangle = new Rectangle( (int) ( ( 画像側終了位置.X - 画像側開始位置.X ) * num5 ), (int) ( ( 画像側終了位置.Y - 画像側開始位置.Y ) * num5 ), ( (int) ( ( 画像側終了位置.X - 画像側開始位置.X ) * num5 ) ) + size5.Width, ( (int) ( ( 画像側終了位置.Y - 画像側開始位置.Y ) * num5 ) ) + size5.Height );
					rectangle2 = new Rectangle( (int) ( ( 表示側終了位置.X - 表示側開始位置.X ) * num5 ), (int) ( ( 表示側終了位置.Y - 表示側開始位置.Y ) * num5 ), ( (int) ( ( 表示側終了位置.X - 表示側開始位置.X ) * num5 ) ) + size5.Width, ( (int) ( ( 表示側終了位置.Y - 表示側開始位置.Y ) * num5 ) ) + size5.Height );
					if( ( ( rectangle.Right <= 0 ) || ( rectangle.Bottom <= 0 ) ) || ( ( rectangle.Left >= size.Width ) || ( rectangle.Top >= size.Height ) ) )
					{
						return 0;
					}
					if( ( ( rectangle2.Right <= 0 ) || ( rectangle2.Bottom <= 0 ) ) || ( ( rectangle2.Left >= size2.Width ) || ( rectangle2.Top >= size2.Height ) ) )
					{
						return 0;
					}
					if( rectangle.X < 0 )
					{
						int num6 = -rectangle.X;
						rectangle2.X += num6;
						rectangle2.Width -= num6;
						rectangle.X = 0;
						rectangle.Width -= num6;
					}
					if( rectangle.Y < 0 )
					{
						int num7 = -rectangle.Y;
						rectangle2.Y += num7;
						rectangle2.Height -= num7;
						rectangle.Y = 0;
						rectangle.Height -= num7;
					}
					if( rectangle.Right > size.Width )
					{
						int num8 = rectangle.Right - size.Width;
						rectangle2.Width -= num8;
						rectangle.Width -= num8;
					}
					if( rectangle.Bottom > size.Height )
					{
						int num9 = rectangle.Bottom - size.Height;
						rectangle2.Height -= num9;
						rectangle.Height -= num9;
					}
					if( rectangle2.X < 0 )
					{
						int num10 = -rectangle2.X;
						rectangle.X += num10;
						rectangle.Width -= num10;
						rectangle2.X = 0;
						rectangle2.Width -= num10;
					}
					if( rectangle2.Y < 0 )
					{
						int num11 = -rectangle2.Y;
						rectangle.Y += num11;
						rectangle.Height -= num11;
						rectangle2.Y = 0;
						rectangle2.Height -= num11;
					}
					if( rectangle2.Right > size2.Width )
					{
						int num12 = rectangle2.Right - size2.Width;
						rectangle.Width -= num12;
						rectangle2.Width -= num12;
					}
					if( rectangle2.Bottom > size2.Height )
					{
						int num13 = rectangle2.Bottom - size2.Height;
						rectangle.Height -= num13;
						rectangle2.Height -= num13;
					}
					if( ( rectangle.X >= rectangle.Right ) || ( rectangle.Y >= rectangle.Bottom ) )
					{
						return 0;
					}
					if( ( rectangle2.X >= rectangle2.Right ) || ( rectangle2.Y >= rectangle2.Bottom ) )
					{
						return 0;
					}
					if( ( ( rectangle.Right < 0 ) || ( rectangle.Bottom < 0 ) ) || ( ( rectangle.X > size.Width ) || ( rectangle.Y > size.Height ) ) )
					{
						return 0;
					}
					if( ( ( rectangle2.Right < 0 ) || ( rectangle2.Bottom < 0 ) ) || ( ( rectangle2.X > size2.Width ) || ( rectangle2.Y > size2.Height ) ) )
					{
						return 0;
					}
				}
				if( ( this.tx描画用 != null ) && ( this.n総移動時間ms != -1 ) )
				{
					if( this.bフレームを作成した && ( this.pBmp != IntPtr.Zero ) )
					{
						DataRectangle rectangle3 = this.tx描画用.texture.LockRectangle( 0, LockFlags.None );
						DataStream data = rectangle3.Data;
						int num14 = rectangle3.Pitch / this.tx描画用.szテクスチャサイズ.Width;
						BitmapUtil.BITMAPINFOHEADER* pBITMAPINFOHEADER = (BitmapUtil.BITMAPINFOHEADER*) this.pBmp.ToPointer();
						if( pBITMAPINFOHEADER->biBitCount == 0x18 )
						{
							switch( num14 )
							{
								case 2:
									this.rAVI.avi.tBitmap24ToGraphicsStreamR5G6B5( pBITMAPINFOHEADER, data, this.tx描画用.szテクスチャサイズ.Width, this.tx描画用.szテクスチャサイズ.Height );
									break;

								case 4:
									this.rAVI.avi.tBitmap24ToGraphicsStreamX8R8G8B8( pBITMAPINFOHEADER, data, this.tx描画用.szテクスチャサイズ.Width, this.tx描画用.szテクスチャサイズ.Height );
									break;
							}
						}
						this.tx描画用.texture.UnlockRectangle( 0 );
						this.bフレームを作成した = false;
					}

					// 旧動画 (278x355以下)の場合と、それ以上の場合とで、拡大/表示位置補正ロジックを変えること。
					// 旧動画の場合は、「278x355の領域に表示される」ことを踏まえて扱う必要あり。
					// 例: 上半分だけ動画表示するような場合は・・・「上半分だけ」という表示意図を維持すべきか？それとも無視して全画面拡大すべきか？？

					float magX, magY;
					int xx, yy;
					if ( ! bFullScreenMovie )
					//if ( bHasBGA || bIsPreviewMovie )
					{
						#region [ BGA領域での再生 ]
						xx = x;
						yy = y;
						magX = Scale.X;
						magY = Scale.Y;
						#endregion
					}
					else
					{
						#region [ 全画面背景再生。アスペクト比を維持した拡大縮小 ]
						xx = 0;
						yy = 0;
						magX = (float) SampleFramework.GameWindowSize.Width / this.rAVI.avi.nフレーム幅;
						magY = (float) SampleFramework.GameWindowSize.Height / this.rAVI.avi.nフレーム高さ;
						if ( magX > magY )
						{
							magX = magY;
							xx = (int) ( ( SampleFramework.GameWindowSize.Width  - ( this.rAVI.avi.nフレーム幅   * magY ) ) / 2 );
						}
						else
						{
							magY = magX;
							yy = (int) ( ( SampleFramework.GameWindowSize.Height - ( this.rAVI.avi.nフレーム高さ * magX ) ) / 2 );
						}
						#endregion
					}

					
					this.tx描画用.vc拡大縮小倍率.X = magX;
					this.tx描画用.vc拡大縮小倍率.Y = magY;
					this.tx描画用.vc拡大縮小倍率.Z = 1.0f;
					this.tx描画用.t2D描画( CDTXMania.app.Device, xx, yy );

					//this.tx描画用.t2D描画( CDTXMania.app.Device, x, y );
				}
			}
			return 0;
		}

		
		// CActivity 実装

		public override void On活性化()
		{
			this.rAVI = null;
			this.n移動開始時刻ms = -1;
			this.n前回表示したフレーム番号 = -1;
			this.bフレームを作成した = false;
			this.pBmp = IntPtr.Zero;
			// this.bIsPreviewMovie = false;	// bIsPreviewMovieは、活性化前にtrueにすること (OnManagedリソースの作成 で参照しているため)
			this.bHasBGA = false;
			this.bFullScreenMovie = false;
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
#if TEST_Direct3D9Ex
				this.tx描画用 = new CTexture( CDTXMania.app.Device,
					320,
					355,
					CDTXMania.app.GraphicsDeviceManager.CurrentSettings.BackBufferFormat, Pool.Default, Usage.Dynamic );
#else
				try
				{
					this.tx描画用 = new CTexture( CDTXMania.app.Device,
						//(bIsPreviewMovie)? 204 : 278,
						//(bIsPreviewMovie)? 269 : 355,
						( bIsPreviewMovie ) ? 204 : SampleFramework.GameWindowSize.Width,
						( bIsPreviewMovie ) ? 269 : SampleFramework.GameWindowSize.Height,
						CDTXMania.app.GraphicsDeviceManager.CurrentSettings.BackBufferFormat, Pool.Managed );
				}
				catch ( CTextureCreateFailedException e )
				{
					Trace.TraceError( "CActAVI: OnManagedリソースの作成(): " + e.Message );
					this.tx描画用 = null;
				}
#endif
				this.tx描画用.vc拡大縮小倍率 = new Vector3( Scale.X, Scale.Y, 1f );
				//this.tx描画用.vc拡大縮小倍率 = new Vector3( 2f, 2f, 1f );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				if( this.tx描画用 != null )
				{
					this.tx描画用.Dispose();
					this.tx描画用 = null;
				}
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(int,int)のほうを使用してください。" );
		}


		// その他

		#region [ private ]
		//-----------------
		private bool bフレームを作成した;
		private long n移動開始時刻ms;
		private int n画像側開始位置X;
		private int n画像側開始位置Y;
		private int n画像側終了位置X;
		private int n画像側終了位置Y;
		private int n開始サイズH;
		private int n開始サイズW;
		private int n終了サイズH;
		private int n終了サイズW;
		private int n前回表示したフレーム番号;
		private int n総移動時間ms;
		private int n表示側開始位置X;
		private int n表示側開始位置Y;
		private int n表示側終了位置X;
		private int n表示側終了位置Y;
		private IntPtr pBmp;
		private CDTX.CAVI rAVI;
		private CTexture tx描画用;
		//-----------------
		#endregion
	}
}
