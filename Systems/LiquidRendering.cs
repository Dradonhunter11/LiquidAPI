using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Liquid;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using LiquidAPI.APIs;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Terraria.GameContent;
using ReLogic.Content;

namespace LiquidAPI.Systems
{
	public class LiquidRendering : ModSystem
	{
		public override void Load()
		{
			instance = this;
			if (!Main.dedServ)
			{
				InitiateArrays(LiquidID.Count);
				int[] waterstyles = new int[12] { 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13 };
				int[] liquidTypes = new int[3] { 1, 11, 14 };
				for (int type = 0; type < LiquidID.Count; type++)
				{
					for (int style = 0; style < liquidStyleCount[type]; style++)
					{
						if (type == LiquidID.Water)
						{
							foreach (int i in waterstyles)
							{
								LiquidTextureAssets.Liquid[type][style] = LiquidRenderer.Instance._liquidTextures[i];
							}
						}
						else if (style == 0)
						{
							foreach (int i in liquidTypes)
							{
								LiquidTextureAssets.Liquid[type][style] = LiquidRenderer.Instance._liquidTextures[i];
							}
						}
						else
						{
							LiquidTextureAssets.Liquid[type][style] = TextureAssets.MagicPixel;
						}
					}
				}
			}
		}

		public override void Unload()
		{
			instance = null;
		}

		public static LiquidRendering instance;

		public static float[][] liquidAlpha = new float[LiquidID.Count][];

		public static int[] liquidStyleCount = { 11, 1, 1, 1 };

		/// <summary>
		/// The style of the given liquid. This stores what style is currently loaded for a liquid. <br />
		/// Allows for more than just water to have styles, even for modded liquids <br />
		/// Defaults to 0 (default liquid style)
		/// </summary>
		public static int[] liquidStyle = new int[LiquidID.Count];

		public static void InitiateArrays(int Count)
		{
			liquidAlpha = new float[Count][];
			LiquidTextureAssets.Liquid = new Asset<Texture2D>[Count][];
			LiquidTextureAssets.LiquidBlock = new Asset<Texture2D>[Count][];
			LiquidTextureAssets.LiquidSlope = new Asset<Texture2D>[Count][];
			if (Count <= liquidStyleCount.Length)
			{
				for (int i = 0; i < liquidAlpha.Length; i++)
				{
					liquidAlpha[i] = new float[liquidStyleCount[i]];
				}
				for (int i = 0; i < LiquidTextureAssets.Liquid.Length; i++)
				{
					LiquidTextureAssets.Liquid[i] = new Asset<Texture2D>[liquidStyleCount[i]];
				}
				for (int i = 0; i < LiquidTextureAssets.LiquidBlock.Length; i++)
				{
					LiquidTextureAssets.LiquidBlock[i] = new Asset<Texture2D>[liquidStyleCount[i]];
				}
				for (int i = 0; i < LiquidTextureAssets.LiquidSlope.Length; i++)
				{
					LiquidTextureAssets.LiquidSlope[i] = new Asset<Texture2D>[liquidStyleCount[i]];
				}
			}
			else
			{
				for (int i = 0; i < liquidAlpha.Length; i++)
				{
					liquidAlpha[i] = new float[1];
				}
				for (int i = 0; i < LiquidTextureAssets.Liquid.Length; i++)
				{
					LiquidTextureAssets.Liquid[i] = new Asset<Texture2D>[1];
				}
				for (int i = 0; i < LiquidTextureAssets.LiquidBlock.Length; i++)
				{
					LiquidTextureAssets.LiquidBlock[i] = new Asset<Texture2D>[1];
				}
				for (int i = 0; i < LiquidTextureAssets.LiquidSlope.Length; i++)
				{
					LiquidTextureAssets.LiquidSlope[i] = new Asset<Texture2D>[1];
				}
			}
		}

		public static bool IsLiquidStyleValid(int type, int style)
		{
			if (type == LiquidID.Water)
			{
				return Main.IsLiquidStyleWater(liquidStyle[type]);
			}
			return false;
		}

		public void InitateDrawLiquids(bool isBackground = false)
		{
			//drewLava = false;
			for (int i = 0; i < LiquidLoader.LiquidCount; i++)
			{
				if (!isBackground)
				{
					if (i == LiquidID.Water)
					{
						liquidStyle[i] = Main.waterStyle;
						liquidStyle[i] = Main.CalculateWaterStyle();
					}
					for (int j = 0; j < liquidAlpha.Length; j++)
					{
						if (IsLiquidStyleValid(j, i))
						{
							if (liquidStyle[i] != j)
							{
								liquidAlpha[i][j] = Math.Max(liquidAlpha[i][j] - 0.2f, 0f);
							}
							else
							{
								liquidAlpha[i][j] = Math.Min(liquidAlpha[i][j] + 0.2f, 1f);
							}
						}
					}
					LoaderManager.Get<WaterStylesLoader>().UpdateLiquidAlphas(); //Sets Main.waterStyle to the modded water styles
				}
				if (!Main.drawToScreen && !isBackground)
				{
					Vector2 vector = (Vector2)(Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange));
					int val = (int)((Main.Camera.ScaledPosition.X - vector.X) / 16f - 1f);
					int val2 = (int)((Main.Camera.ScaledPosition.X + Main.Camera.ScaledSize.X + vector.X) / 16f) + 2;
					int val3 = (int)((Main.Camera.ScaledPosition.Y - vector.Y) / 16f - 1f);
					int val4 = (int)((Main.Camera.ScaledPosition.Y + Main.Camera.ScaledSize.Y + vector.Y) / 16f) + 5;
					val = Math.Max(val, 5) - 2;
					val3 = Math.Max(val3, 5);
					val2 = Math.Min(val2, Main.maxTilesX - 5) + 2;
					val4 = Math.Min(val4, Main.maxTilesY - 5) + 4;
					Rectangle drawArea = new(val, val3, val2 - val, val4 - val3);
					LiquidRenderer.Instance.PrepareDraw(drawArea);
				}
				bool flag = false;
				for (int j = 0; j < LoaderManager.Get<WaterStylesLoader>().TotalCount; j++)
				{
					if (IsLiquidStyleValid(i, j) && liquidAlpha[i][j] > 0f && j != liquidStyle[i])
					{
						DrawLiquid(i, isBackground, j, isBackground ? 1f : liquidAlpha[i][j], drawSinglePassLiquids: false);
						flag = true;
					}
				}
				DrawLiquid(i, isBackground, liquidStyle[i], flag ? liquidAlpha[i][liquidStyle[i]] : 1f);
			}
		}

		protected internal void DrawLiquid(int type, bool bg = false, int style = 0, float Alpha = 1f, bool drawSinglePassLiquids = true)
		{
			//if (!Lighting.NotRetro) //I am NOT happy that we aren't keeping retro lighting
			//{
			//	oldDrawWater(bg, waterStyle, Alpha);
			//	return;
			//}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Vector2 drawOffset = (Vector2)(Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange)) - Main.screenPosition;
			if (bg)
			{
				//Main.instance.TilesRenderer.DrawLiquidBehindTiles(waterStyle); //unimplemented
			}
			DrawNormalLiquids(Main.spriteBatch, type, style, drawOffset, Alpha, bg);
			if (drawSinglePassLiquids)
			{
				if (type == LiquidID.Shimmer)
				{
					DrawShimmer(Main.spriteBatch, type, style, drawOffset, bg);
				}
				//Give mods the ability to allow single pass drawing? (whatever that means)
			}
			if (!bg)
			{
				TimeLogger.DrawTime(4, stopwatch.Elapsed.TotalMilliseconds);
			}
		}

		public unsafe void DrawNormalLiquids(SpriteBatch spriteBatch, int type, int style, Vector2 drawOffset, float globalAlpha, bool isBackgroundDraw)
		{
			Rectangle drawArea = LiquidRenderer.Instance._drawArea;
			Main.tileBatch.Begin();
			fixed (LiquidRenderer.LiquidDrawCache* ptr3 = &LiquidRenderer.Instance._drawCache[0])
			{
				LiquidRenderer.LiquidDrawCache* ptr2 = ptr3;
				for (int i = drawArea.X; i < drawArea.X + drawArea.Width; i++)
				{
					for (int j = drawArea.Y; j < drawArea.Y + drawArea.Height; j++)
					{
						if (ptr2->IsVisible)
						{
							Rectangle sourceRectangle = ptr2->SourceRectangle;
							if (ptr2->IsSurfaceLiquid)
							{
								sourceRectangle.Y = 1280;
							}
							else
							{
								sourceRectangle.Y += LiquidRenderer.Instance._animationFrame * 80;
							}
							Vector2 liquidOffset = ptr2->LiquidOffset;
							float num = ptr2->Opacity * (isBackgroundDraw ? 1f : LiquidRenderer.DEFAULT_OPACITY[type]);
							num *= globalAlpha;
							num = Math.Min(1f, num);
							Lighting.GetCornerColors(i, j, out var vertices);
							ref Color bottomLeftColor = ref vertices.BottomLeftColor;
							bottomLeftColor *= num;
							ref Color bottomRightColor = ref vertices.BottomRightColor;
							bottomRightColor *= num;
							ref Color topLeftColor = ref vertices.TopLeftColor;
							topLeftColor *= num;
							ref Color topRightColor = ref vertices.TopRightColor;
							topRightColor *= num;
							Main.DrawTileInWater(drawOffset, i, j);
							Main.tileBatch.Draw(LiquidTextureAssets.Liquid[type][style].Value, new Vector2((float)(i << 4), (float)(j << 4)) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, (SpriteEffects)0);
						}
						ptr2++;
					}
				}
			}
			Main.tileBatch.End();
		}

		public unsafe void DrawShimmer(SpriteBatch spriteBatch, int type, int style, Vector2 drawOffset, bool isBackgroundDraw)
		{
			Rectangle drawArea = LiquidRenderer.Instance._drawArea;
			Main.tileBatch.Begin();
			fixed (LiquidRenderer.SpecialLiquidDrawCache* ptr3 = &LiquidRenderer.Instance._drawCacheForShimmer[0])
			{
				LiquidRenderer.SpecialLiquidDrawCache* ptr2 = ptr3;
				int num = LiquidRenderer.Instance._drawCacheForShimmer.Length;
				for (int i = 0; i < num; i++)
				{
					if (!ptr2->IsVisible || type != LiquidID.Shimmer)
					{
						break;
					}
					Rectangle sourceRectangle = ptr2->SourceRectangle;
					if (ptr2->IsSurfaceLiquid)
					{
						sourceRectangle.Y = 1280;
					}
					else
					{
						sourceRectangle.Y += LiquidRenderer.Instance._animationFrame * 80;
					}
					Vector2 liquidOffset = ptr2->LiquidOffset;
					float val = ptr2->Opacity * (isBackgroundDraw ? 1f : 0.75f);
					val = Math.Min(1f, val);
					int num3 = ptr2->X + drawArea.X - 2;
					int num4 = ptr2->Y + drawArea.Y - 2;
					Lighting.GetCornerColors(num3, num4, out var vertices);
					LiquidRenderer.SetShimmerVertexColors(ref vertices, val, num3, num4);
					Main.DrawTileInWater(drawOffset, num3, num4);
					Main.tileBatch.Draw(LiquidTextureAssets.Liquid[type][style].Value, new Vector2((float)(num3 << 4), (float)(num4 << 4)) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, (SpriteEffects)0);
					sourceRectangle = ptr2->SourceRectangle;
					bool flag = sourceRectangle.X != 16 || sourceRectangle.Y % 80 != 48;
					if (flag || (num3 + num4) % 2 == 0)
					{
						sourceRectangle.X += 48;
						sourceRectangle.Y += 80 * LiquidRenderer.Instance.GetShimmerFrame(flag, num3, num4);
						LiquidRenderer.SetShimmerVertexColors_Sparkle(ref vertices, ptr2->Opacity, num3, num4, flag);
						Main.tileBatch.Draw(LiquidTextureAssets.Liquid[type][style].Value, new Vector2((float)(num3 << 4), (float)(num4 << 4)) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, (SpriteEffects)0);
					}
					ptr2++;
				}
			}
			Main.tileBatch.End();
		}
	}
}
