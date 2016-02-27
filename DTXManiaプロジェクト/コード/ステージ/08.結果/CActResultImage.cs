﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;

namespace DTXMania
{
	internal class CActResultImage : CActivity
	{
		// コンストラクタ

		public CActResultImage()
		{
			base.b活性化してない = true;
		}


		// メソッド

		public void tアニメを完了させる()
		{
			this.ct登場用.n現在の値 = this.ct登場用.n終了値;
		}
		public CAct演奏AVI actAVI
		{
			get;
			set;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.n本体X = 4;
			this.n本体Y = 0x3f;
			base.On活性化();
			this.actAVI.bIsPreviewMovie = true;
			this.actAVI.On活性化();
		}
		public override void On非活性化()
		{
			if (this.ct登場用 != null)
			{
				this.ct登場用 = null;
			}
			if (this.rAVI != null)
			{
				this.rAVI.Dispose();
				this.rAVI = null;
			}
			base.On非活性化();
			this.actAVI.On非活性化();

		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txパネル本体 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult resultimage panel.png"));
				this.txリザルト画像がないときの画像 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect preimage default.png"));
				//this.sfリザルトAVI画像 = Surface.CreateOffscreenPlain( CDTXMania.Instance.app.Device, 0xcc, 0x10d, CDTXMania.Instance.app.GraphicsDeviceManager.CurrentSettings.BackBufferFormat, Pool.Default );
				//this.sfリザルトAVI画像 = Surface.CreateOffscreenPlain( CDTXMania.Instance.app.Device, 192, 269, CDTXMania.Instance.app.GraphicsDeviceManager.CurrentSettings.BackBufferFormat, Pool.Default );
				//this.nAVI再生開始時刻 = -1;
				//this.n前回描画したフレーム番号 = -1;
				//this.b動画フレームを作成した = false;
				//this.pAVIBmp = IntPtr.Zero;
				base.OnManagedリソースの作成();
				this.actAVI.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txパネル本体);
				TextureFactory.tテクスチャの解放(ref this.txリザルト画像);
				TextureFactory.tテクスチャの解放(ref this.txリザルト画像がないときの画像);
				//if( this.sfリザルトAVI画像 != null )
				//{
				//    this.sfリザルトAVI画像.Dispose();
				//    this.sfリザルトAVI画像 = null;
				//}
				base.OnManagedリソースの解放();
				this.actAVI.OnManagedリソースの解放();
			}
		}
		public override unsafe int On進行描画()
		{
			if (base.b活性化してない)
			{
				return 0;
			}
			if (base.b初めての進行描画)
			{
				if (CDTXMania.Instance.ConfigIni.bストイックモード)
				{
					this.r表示するリザルト画像 = this.txリザルト画像がないときの画像;
				}
				else if (((!this.tリザルト動画の指定があれば構築する() && !this.tリザルト画像の指定があれば構築する()) && (!this.tプレビュー動画の指定があれば構築する() && !this.tプレビュー画像の指定があれば構築する())) && !this.t背景画像があればその一部からリザルト画像を構築する())
				{
					this.r表示するリザルト画像 = this.txリザルト画像がないときの画像;
				}

				this.ct登場用 = new CCounter(0, 100, 5, CDTXMania.Instance.Timer);
				base.b初めての進行描画 = false;
			}
			this.ct登場用.t進行();
			//if ( ( ( this.avi != null ) && ( this.sfリザルトAVI画像 != null ) ) && ( this.nAVI再生開始時刻 != -1 ) )
			//if ( ( this.avi != null ) && ( this.nAVI再生開始時刻 != -1 ) )
			//{
			//    int time = (int) ( ( CDTXMania.Instance.Timer.n現在時刻 - this.nAVI再生開始時刻 ) * ( ( (double) CDTXMania.Instance.ConfigIni.n演奏速度 ) / 20.0 ) );
			//    int frameNoFromTime = this.avi.GetFrameNoFromTime( time );
			//    if( frameNoFromTime >= this.avi.GetMaxFrameCount() )
			//    {
			//        this.nAVI再生開始時刻 = CDTXMania.Instance.Timer.n現在時刻;
			//    }
			//    else if( ( this.n前回描画したフレーム番号 != frameNoFromTime ) && !this.b動画フレームを作成した )
			//    {
			//        this.b動画フレームを作成した = true;
			//        this.n前回描画したフレーム番号 = frameNoFromTime;
			//        //this.pAVIBmp = this.avi.GetFramePtr( frameNoFromTime );
			//    }
			//}
			if (this.ct登場用.b終了値に達した)
			{
				this.n本体X = 4;
				this.n本体Y = 0x3f;
			}
			else
			{
				double num3 = ((double)this.ct登場用.n現在の値) / 100.0;
				double num4 = Math.Cos((1.5 + (0.5 * num3)) * Math.PI);
				this.n本体X = 4;
				this.n本体Y = 0x3f - ((int)(((this.txパネル本体 != null) ? ((double)this.txパネル本体.sz画像サイズ.Height) : ((double)0)) * (1.0 - (num4 * num4))));
			}
			if (this.txパネル本体 != null)
			{
				this.txパネル本体.t2D描画(
					CDTXMania.Instance.Device,
					this.n本体X * Scale.X,
					this.n本体Y * Scale.Y
				);
			}
			int x = this.n本体X + 0x11;
			int y = this.n本体Y + 0x10;
			//if ( ( ( this.nAVI再生開始時刻 != -1 ) && ( this.avi != null ) ) && ( this.sfリザルトAVI画像 != null ) )
			if (this.rAVI != null)
			{
				this.actAVI.t進行描画((int)(x * Scale.X), (int)(y * Scale.Y), 612, 605);
			}
			#region [ プレビュー画像表示 ]
			else if (this.r表示するリザルト画像 != null)
			{
				CPreviewMagnifier cmg = new CPreviewMagnifier();
				cmg.GetMagnifier(this.r表示するリザルト画像.sz画像サイズ.Width, this.r表示するリザルト画像.sz画像サイズ.Height, 1.0f, 1.0f, true);

				if (cmg.width < 0xcc) x += (204 - cmg.width) / 2;
				if (cmg.height < 269) y += (269 - cmg.height) / 2;
				this.r表示するリザルト画像.vc拡大縮小倍率.X = cmg.magX;
				this.r表示するリザルト画像.vc拡大縮小倍率.Y = cmg.magY;
				this.r表示するリザルト画像.vc拡大縮小倍率.Z = 1f;
				this.r表示するリザルト画像.t2D描画(CDTXMania.Instance.Device, x * Scale.X, y * Scale.Y);
			}
			#endregion
			if ((CDTXMania.Instance.DTX.GENRE != null) && (CDTXMania.Instance.DTX.GENRE.Length > 0))
			{
				CDTXMania.Instance.act文字コンソール.tPrint(
					(int)((this.n本体X + 0x12) * Scale.X),
					(int)((this.n本体Y - 1) * Scale.Y),
					C文字コンソール.Eフォント種別.赤細,
					CDTXMania.Instance.DTX.GENRE
				);
			}
			if (!this.ct登場用.b終了値に達した)
			{
				return 0;
			}
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		//private CAvi avi;
		private CDTX.CAVI rAVI;
		//private bool b動画フレームを作成した;
		private CCounter ct登場用;
		//private long nAVI再生開始時刻;
		//private int n前回描画したフレーム番号;
		private int n本体X;
		private int n本体Y;
		//private IntPtr pAVIBmp;
		private CTexture r表示するリザルト画像;
		//private Surface sfリザルトAVI画像;
		private string strAVIファイル名;
		private CTexture txパネル本体;
		private CTexture txリザルト画像;
		private CTexture txリザルト画像がないときの画像;

		private bool t背景画像があればその一部からリザルト画像を構築する()
		{
			string bACKGROUND;
			if ((CDTXMania.Instance.ConfigIni.bギタレボモード && (CDTXMania.Instance.DTX.BACKGROUND_GR != null)) && (CDTXMania.Instance.DTX.BACKGROUND_GR.Length > 0))
			{
				bACKGROUND = CDTXMania.Instance.DTX.BACKGROUND_GR;
			}
			else
			{
				bACKGROUND = CDTXMania.Instance.DTX.BACKGROUND;
			}
			if (string.IsNullOrEmpty(bACKGROUND))
			{
				return false;
			}
			TextureFactory.tテクスチャの解放(ref this.txリザルト画像);
			this.r表示するリザルト画像 = null;
			bACKGROUND = CDTXMania.Instance.DTX.strフォルダ名 + bACKGROUND;
			Bitmap image = null;
			Bitmap bitmap2 = null;
			Bitmap bitmap3 = null;
			try
			{
				image = new Bitmap(bACKGROUND);
				bitmap2 = new Bitmap(SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height);
				Graphics graphics = Graphics.FromImage(bitmap2);
				int x = 0;
				for (int i = 0; i < SampleFramework.GameWindowSize.Height; i += image.Height)
				{
					for (x = 0; x < SampleFramework.GameWindowSize.Width; x += image.Width)
					{
						graphics.DrawImage(image, x, i, image.Width, image.Height);
					}
				}
				graphics.Dispose();
				bitmap3 = new Bitmap(0xcc, 0x10d);
				graphics = Graphics.FromImage(bitmap3);
				graphics.DrawImage(bitmap2, 5, 5, new Rectangle(0x157, 0x6d, 0xcc, 0x10d), GraphicsUnit.Pixel);
				graphics.Dispose();
				this.txリザルト画像 = new CTexture(CDTXMania.Instance.Device, bitmap3, CDTXMania.Instance.TextureFormat);
				this.r表示するリザルト画像 = this.txリザルト画像;
			}
			catch
			{
				Trace.TraceError("背景画像の読み込みに失敗しました。({0})", new object[] { bACKGROUND });
				this.txリザルト画像 = null;
				return false;
			}
			finally
			{
				if (image != null)
				{
					image.Dispose();
				}
				if (bitmap2 != null)
				{
					bitmap2.Dispose();
				}
				if (bitmap3 != null)
				{
					bitmap3.Dispose();
				}
			}
			return true;
		}
		//private unsafe void tサーフェイスをクリアする( Surface sf )
		//{
		//    DataRectangle rectangle = sf.LockRectangle( LockFlags.None );
		//    DataStream data = rectangle.Data;
		//    switch ( ( rectangle.Pitch / sf.Description.Width ) )
		//    {
		//        case 4:
		//            {
		//                uint* numPtr = (uint*) data.DataPointer.ToPointer();
		//                for ( int i = 0; i < sf.Description.Height; i++ )
		//                {
		//                    for ( int j = 0; j < sf.Description.Width; j++ )
		//                    {
		//                        ( numPtr + ( i * sf.Description.Width ) )[ j ] = 0;
		//                    }
		//                }
		//                break;
		//            }
		//        case 2:
		//            {
		//                ushort* numPtr2 = (ushort*) data.DataPointer.ToPointer();
		//                for ( int k = 0; k < sf.Description.Height; k++ )
		//                {
		//                    for ( int m = 0; m < sf.Description.Width; m++ )
		//                    {
		//                        ( numPtr2 + ( k * sf.Description.Width ) )[ m ] = 0;
		//                    }
		//                }
		//                break;
		//            }
		//    }
		//    sf.UnlockRectangle();
		//}
		private bool tプレビュー画像の指定があれば構築する()
		{
			if (string.IsNullOrEmpty(CDTXMania.Instance.DTX.PREIMAGE))
			{
				return false;
			}
			TextureFactory.tテクスチャの解放(ref this.txリザルト画像);
			this.r表示するリザルト画像 = null;
			string path = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.PREIMAGE;
			if (!File.Exists(path))
			{
				Trace.TraceWarning("ファイルが存在しません。({0})", new object[] { path });
				return false;
			}
			this.txリザルト画像 = TextureFactory.tテクスチャの生成(path);
			this.r表示するリザルト画像 = this.txリザルト画像;
			return (this.r表示するリザルト画像 != null);
		}
		private bool tプレビュー動画の指定があれば構築する()
		{
			if (!CDTXMania.Instance.ConfigIni.bAVI有効)
			{
				return false;
			}
			this.actAVI.Stop();
			if (string.IsNullOrEmpty(CDTXMania.Instance.DTX.PREMOVIE))
			{
				return false;
			}
			this.strAVIファイル名 = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.PREMOVIE;
			if (!File.Exists(this.strAVIファイル名))
			{
				Trace.TraceWarning("プレビュー動画のファイルが存在しません。({0})", this.strAVIファイル名);
				return false;
			}
			if (this.rAVI != null)
			{
				this.rAVI.Dispose();
				this.rAVI = null;
			}
			try
			{
				this.rAVI = new CDTX.CAVI(00, this.strAVIファイル名, "", CDTXMania.Instance.ConfigIni.n演奏速度);
				this.rAVI.OnDeviceCreated();
				this.actAVI.Start(Ech定義.Movie, rAVI, 204, 269, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1);

				//this.avi = new CAvi( this.strAVIファイル名 );
				//this.nAVI再生開始時刻 = CDTXMania.Instance.Timer.n現在時刻;
				//this.n前回描画したフレーム番号 = -1;
				//this.b動画フレームを作成した = false;
				//this.tサーフェイスをクリアする( this.sfリザルトAVI画像 );
				//Trace.TraceInformation( "プレビュー動画を生成しました。({0})", this.strAVIファイル名 );
			}
			catch
			{
				Trace.TraceError("プレビュー動画の生成に失敗しました。({0})", this.strAVIファイル名);
				this.rAVI = null;
				//this.nAVI再生開始時刻 = -1;
			}
			return true;
		}
		private bool tリザルト画像の指定があれば構築する()
		{
			int rank = CScoreIni.t総合ランク値を計算して返す(CDTXMania.Instance.stage結果.st演奏記録);
			if (rank == 99)	// #23534 2010.10.28 yyagi: 演奏チップが0個のときは、rankEと見なす
			{
				rank = 6;
			}
			if (string.IsNullOrEmpty(CDTXMania.Instance.DTX.RESULTIMAGE[rank]))
			{
				return false;
			}
			TextureFactory.tテクスチャの解放(ref this.txリザルト画像);
			this.r表示するリザルト画像 = null;
			string path = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.RESULTIMAGE[rank];
			if (!File.Exists(path))
			{
				Trace.TraceWarning("ファイルが存在しません。({0})", new object[] { path });
				return false;
			}
			this.txリザルト画像 = TextureFactory.tテクスチャの生成(path);
			this.r表示するリザルト画像 = this.txリザルト画像;
			return (this.r表示するリザルト画像 != null);
		}
		private bool tリザルト動画の指定があれば構築する()
		{
			if (!CDTXMania.Instance.ConfigIni.bAVI有効)
			{
				return false;
			}
			int rank = CScoreIni.t総合ランク値を計算して返す(CDTXMania.Instance.stage結果.st演奏記録);
			if (rank == 99)	// #23534 2010.10.28 yyagi: 演奏チップが0個のときは、rankEと見なす
			{
				rank = 6;
			}

			if (string.IsNullOrEmpty(CDTXMania.Instance.DTX.RESULTMOVIE[rank]))
			{
				return false;
			}
			this.strAVIファイル名 = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.RESULTMOVIE[rank];
			if (!File.Exists(this.strAVIファイル名))
			{
				Trace.TraceWarning("リザルト動画のファイルが存在しません。({0})", this.strAVIファイル名);
				return false;
			}
			if (this.rAVI != null)
			{
				this.rAVI.Dispose();
				this.rAVI = null;
			}
			try
			{
				this.rAVI = new CDTX.CAVI(00, this.strAVIファイル名, "", CDTXMania.Instance.ConfigIni.n演奏速度);
				this.rAVI.OnDeviceCreated();
				this.actAVI.Start(Ech定義.Movie, rAVI, 204, 269, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1);

				//this.avi = new CAvi( this.strAVIファイル名 );
				//this.nAVI再生開始時刻 = CDTXMania.Instance.Timer.n現在時刻;
				//this.n前回描画したフレーム番号 = -1;
				//this.b動画フレームを作成した = false;
				//this.tサーフェイスをクリアする( this.sfリザルトAVI画像 );
				//Trace.TraceInformation( "リザルト動画を生成しました。({0})", this.strAVIファイル名 );
			}
			catch
			{
				Trace.TraceError("リザルト動画の生成に失敗しました。({0})", this.strAVIファイル名);
				this.rAVI = null;
				//this.nAVI再生開始時刻 = -1;
			}
			return true;
		}
		//-----------------
		#endregion
	}
}
