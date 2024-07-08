using CorePatcher;
using LiquidAPI.Patches;
using LiquidAPI.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LiquidAPI.APIs
{
    public abstract class ModLiquid : ModBlockType
    {
        private int waterfallLength = 10;
        private float defaultOpacity = 0.6f;

        public int WaterfallLength
        {
            get => LiquidRenderer.WATERFALL_LENGTH[Type];
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Waterfall Length must not be inferior to 0.");
                LiquidRenderer.WATERFALL_LENGTH[Type] = value;
            }
        }

        public float DefaultOpacity
        {
            get => LiquidRenderer.DEFAULT_OPACITY[Type];
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Default opacity must be between 0 and 1.");
                }
                LiquidRenderer.DEFAULT_OPACITY[Type] = value;
            }
        }

        public byte WaveMaskStrength
        {
            get => LiquidRenderer.WAVE_MASK_STRENGTH[Type + 1];
            set => LiquidRenderer.WAVE_MASK_STRENGTH[Type + 1] = value;
        }

        public byte ViscosityMask
        {
            get => LiquidRenderer.VISCOSITY_MASK[Type + 1];
            set => LiquidRenderer.VISCOSITY_MASK[Type + 1] = value;
        }

        public override string LocalizationCategory => "Liquids";

        public sealed override void SetupContent()
        {
            if(!PatchLoader.DetectPatchedAssembly()) return;
			LiquidRendering.InitiateArrays(Type + 1);
			LiquidTextureAssets.Liquid[Type][0] = ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2);
			SetStaticDefaults();
            ModLiquidID.Search.Add(FullName, Type);
			//LiquidTextureAssets.LiquidSlope[Type][0] = ModContent.Request<Texture2D>(BlockTextur, (AssetRequestMode)2);
			//LiquidTextureAssets.LiquidBlock[Type][0] = ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2);
		}

        public override void SetStaticDefaults()
        {
            if (!PatchLoader.DetectPatchedAssembly()) return;
            WaterfallLength = 10;
            DefaultOpacity = 0.6f;
        }

        public sealed override void Register()
        {
            if (!PatchLoader.DetectPatchedAssembly()) return;
            ModTypeLookup<ModLiquid>.Register(this);
            Type = (ushort)LiquidLoader.ReserveLiquidID();
            LiquidLoader.liquids.Add(this);
        }

        public void AddMapEntry(Color color, LocalizedText name, Func<string, int, int, string> nameFunc)
        {
            if(!MapLoader.initialized)
            {
                MapEntry entry = new MapEntry(color, name, nameFunc);
                if(!LiquidLoader.liquidEntries.Keys.Contains(Type))
                {
                    LiquidLoader.liquidEntries[Type] = new List<MapEntry>();
                }
                LiquidLoader.liquidEntries[Type].Add(entry);
            }
        }

        internal static TypeDefinition GetTypeDefinition()
        {
            return LiquidLoaderSystems.LiquidAPIDef.MainModule.Types.First(i => i.FullName == "LiquidAPI.APIs.ModLiquid");
        }
    }
}
