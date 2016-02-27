﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SlimDX;

namespace DTXMania
{
	internal class CAct演奏Guitar判定文字列 : CAct演奏判定文字列共通
	{
		// コンストラクタ

		public CAct演奏Guitar判定文字列()
		{
			this.stレーンサイズ = new STレーンサイズ[12];
			STレーンサイズ stレーンサイズ = new STレーンサイズ();
			int[,] sizeXW = new int[,] {
				{ 0x24, 0x24 },
				{ 0x4d, 30 },
				{ 0x6f, 30 },
				{ 0x92, 0x2a },
				{ 0xc1, 30 },
				{ 0xe3, 30 },
				{ 0x105, 30 },
				{ 0x127, 0x24 },
				{ 0, 0 },
				{ 0, 0 },
				{ 0x1a, 0x6f },		// 最後2つ(Gt, Bs)がドラムスと異なる
				{ 480, 0x6f }		// 
			};
			for (int i = 0; i < 12; i++)
			{
				this.stレーンサイズ[i] = new STレーンサイズ();
				this.stレーンサイズ[i].x = sizeXW[i, 0];
				this.stレーンサイズ[i].w = sizeXW[i, 1];
			}
			base.b活性化してない = true;
		}

		// CActivity 実装（共通クラスからの差分のみ）

		public override int On進行描画()
		{
			throw new InvalidOperationException("t進行描画(C演奏判定ライン座標共通 演奏判定ライン共通 ) のほうを使用してください。");
		}
		public override int t進行描画(C演奏判定ライン座標共通 演奏判定ライン座標)
		{
			if (!base.b活性化してない)
			{
				for (int i = 0; i < 12; i++)
				{
					if (!base.st状態[i].ct進行.b停止中)
					{
						base.st状態[i].ct進行.t進行();
						if (base.st状態[i].ct進行.b終了値に達した)
						{
							base.st状態[i].ct進行.t停止();
						}
						int num2 = base.st状態[i].ct進行.n現在の値;
						if ((base.st状態[i].judge != E判定.Miss) && (base.st状態[i].judge != E判定.Bad))
						{
							if (num2 < 50)
							{
								base.st状態[i].fX方向拡大率 = 1f + (1f * (1f - (((float)num2) / 50f)));
								base.st状態[i].fY方向拡大率 = ((float)num2) / 50f;
								base.st状態[i].n相対X座標 = 0;
								base.st状態[i].n相対Y座標 = 0;
								base.st状態[i].n透明度 = 0xff;
							}
							else if (num2 < 130)
							{
								base.st状態[i].fX方向拡大率 = 1f;
								base.st状態[i].fY方向拡大率 = 1f;
								base.st状態[i].n相対X座標 = 0;
								base.st状態[i].n相対Y座標 = ((num2 % 6) == 0) ? (CDTXMania.Instance.Random.Next(6) - 3) : base.st状態[i].n相対Y座標;
								base.st状態[i].n透明度 = 0xff;
							}
							else if (num2 >= 240)
							{
								base.st状態[i].fX方向拡大率 = 1f;
								base.st状態[i].fY方向拡大率 = 1f - ((1f * (num2 - 240)) / 60f);
								base.st状態[i].n相対X座標 = 0;
								base.st状態[i].n相対Y座標 = 0;
								base.st状態[i].n透明度 = 0xff;
							}
							else
							{
								base.st状態[i].fX方向拡大率 = 1f;
								base.st状態[i].fY方向拡大率 = 1f;
								base.st状態[i].n相対X座標 = 0;
								base.st状態[i].n相対Y座標 = 0;
								base.st状態[i].n透明度 = 0xff;
							}
						}
						else if (num2 < 50)
						{
							base.st状態[i].fX方向拡大率 = 1f;
							base.st状態[i].fY方向拡大率 = ((float)num2) / 50f;
							base.st状態[i].n相対X座標 = 0;
							base.st状態[i].n相対Y座標 = 0;
							base.st状態[i].n透明度 = 0xff;
						}
						else if (num2 >= 200)
						{
							base.st状態[i].fX方向拡大率 = 1f - (((float)(num2 - 200)) / 100f);
							base.st状態[i].fY方向拡大率 = 1f - (((float)(num2 - 200)) / 100f);
							base.st状態[i].n相対X座標 = 0;
							base.st状態[i].n相対Y座標 = 0;
							base.st状態[i].n透明度 = 0xff;
						}
						else
						{
							base.st状態[i].fX方向拡大率 = 1f;
							base.st状態[i].fY方向拡大率 = 1f;
							base.st状態[i].n相対X座標 = 0;
							base.st状態[i].n相対Y座標 = 0;
							base.st状態[i].n透明度 = 0xff;
						}
					}
				}
				for (int j = 0; j < 12; j++)
				{
					if (!base.st状態[j].ct進行.b停止中)
					{
						int index = base.st判定文字列[(int)base.st状態[j].judge].n画像番号;
						int baseX = 0;
						int baseY = 0;
						if (j >= 8)
						{
							if (j == 11)		// Bass
							{
								if (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Bass) == E判定文字表示位置.表示OFF)
								{
									continue;
								}
								int yB;
								switch (CDTXMania.Instance.ConfigIni.判定文字表示位置.Bass)
								{
									case E判定文字表示位置.コンボ下:
										baseX = 0x163;
										baseY = CDTXMania.Instance.ConfigIni.bReverse.Bass ? 0x12b : 190;
										break;
									case E判定文字表示位置.レーン上:
										baseX = this.stレーンサイズ[j].x;
										yB = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, CDTXMania.Instance.ConfigIni.bReverse.Bass);
										baseY = CDTXMania.Instance.ConfigIni.bReverse.Bass ? yB - 95 - 52 + 10 : yB + 95 + 52;
										//baseY = CDTXMania.Instance.ConfigIni.bReverse.Bass ? 0x12b : 190;
										break;
									case E判定文字表示位置.判定ライン上:
										baseX = this.stレーンサイズ[j].x;
										yB = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, CDTXMania.Instance.ConfigIni.bReverse.Bass);
										baseY = CDTXMania.Instance.ConfigIni.bReverse.Bass ? yB + 30 : yB - 20;
										break;
								}
							}
							else if (j == 10)	// Guitar
							{
								if (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Guitar) == E判定文字表示位置.表示OFF)
								{
									continue;
								}
								int yG;
								switch (CDTXMania.Instance.ConfigIni.判定文字表示位置.Guitar)
								{
									case E判定文字表示位置.コンボ下:
										baseX = 0xaf;
										baseY = CDTXMania.Instance.ConfigIni.bReverse.Guitar ? 0x12b : 190;
										break;
									case E判定文字表示位置.レーン上:
										baseX = this.stレーンサイズ[j].x;
										yG = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, CDTXMania.Instance.ConfigIni.bReverse.Guitar);
										baseY = CDTXMania.Instance.ConfigIni.bReverse.Guitar ? yG - 95 - 52 + 10 : yG + 95 + 52;
										//baseY = CDTXMania.Instance.ConfigIni.bReverse.Guitar ? 0x12b : 190;
										break;
									case E判定文字表示位置.判定ライン上:
										baseX = this.stレーンサイズ[j].x;
										yG = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, CDTXMania.Instance.ConfigIni.bReverse.Guitar);
										baseY = CDTXMania.Instance.ConfigIni.bReverse.Guitar ? yG + 30 : yG - 20;
										break;
								}
							}
							//int xc = ( ( baseX + base.st状態[ j ].n相対X座標 ) + ( this.stレーンサイズ[ j ].w / 2 ) );
							//int x = xc - ( (int) ( ( ( 128f * base.st状態[ j ].fX方向拡大率 ) * 0.8 ) / 2.0 ) );
							//int y = ( baseY + base.st状態[ j ].n相対Y座標 ) - ( (int) ( ( ( 43f * base.st状態[ j ].fY方向拡大率 ) * 0.8 ) / 2.0 ) );

							int xc = (int)((((baseX + base.st状態[j].n相対X座標) + (this.stレーンサイズ[j].w / 2))) * Scale.X);
							int x = xc - (int)(((256f / 2) * base.st状態[j].fX方向拡大率) * 0.8);
							int y = (int)((baseY + base.st状態[j].n相対Y座標) * Scale.Y) - ((int)((((256f / 3) * base.st状態[j].fY方向拡大率) * 0.8) / 2.0));

							if (base.tx判定文字列[index] != null)
							{
								base.tx判定文字列[index].n透明度 = base.st状態[j].n透明度;
								base.tx判定文字列[index].vc拡大縮小倍率 = new Vector3((float)(base.st状態[j].fX方向拡大率 * 0.8), (float)(base.st状態[j].fY方向拡大率 * 0.8), 1f);
								base.tx判定文字列[index].t2D描画(CDTXMania.Instance.Device, x, y, base.st判定文字列[(int)base.st状態[j].judge].rc);

								#region [ #25370 2011.6.3 yyagi ShowLag support ]
								if (base.nShowLagType == (int)EShowLagType.ON ||
									 ((base.nShowLagType == (int)EShowLagType.GREAT_POOR) && (base.st状態[j].judge != E判定.Perfect)))
								{
									if (base.st状態[j].judge != E判定.Auto && base.txlag数値 != null)		// #25370 2011.2.1 yyagi
									{
										bool minus = false;
										int offsetX = 0;
										string strDispLag = base.st状態[j].nLag.ToString();
										if (st状態[j].nLag < 0)
										{
											minus = true;
										}
										x = xc - (int)(strDispLag.Length * 15 / 2 * Scale.X);
										for (int i = 0; i < strDispLag.Length; i++)
										{
											int p = (strDispLag[i] == '-') ? 11 : (int)(strDispLag[i] - '0');	//int.Parse(strDispLag[i]);
											p += minus ? 0 : 12;		// change color if it is minus value
											base.txlag数値.t2D描画(
												CDTXMania.Instance.Device,
												x + offsetX * Scale.X,
												y + 35 * Scale.Y,
												base.stLag数値[p].rc
											);
											offsetX += 12;
										}
									}
								}
								#endregion
							}
							// Label_06B7: ;
						}
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout(LayoutKind.Sequential)]
		private struct STレーンサイズ
		{
			public int x;
			public int w;
		}

		private STレーンサイズ[] stレーンサイズ;
		//-----------------
		#endregion
	}
}
