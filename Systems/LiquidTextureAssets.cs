using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace LiquidAPI.Systems
{
	public static class LiquidTextureAssets
	{
		public static Asset<Texture2D>[][] Liquid = new Asset<Texture2D>[4][]; //Format goes as Assets/Misc/liquid_{liquidID}_{LiquidStyle} (having just liquid_{LiquidID} will default to the first style slot)

		public static Asset<Texture2D>[][] LiquidBlock = new Asset<Texture2D>[4][];

		public static Asset<Texture2D>[][] LiquidSlope = new Asset<Texture2D>[4][];
	}
}
