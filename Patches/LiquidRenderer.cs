using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorePatcher;
using CorePatcher.Attributes;
using CorePatcher.Utilities;
using LiquidAPI.Systems;
using Mono.Cecil;
using MonoMod.Cil;

namespace LiquidAPI.Patches
{
    [PatchType("Terraria.GameContent.Liquid.LiquidRenderer")]
    internal class LiquidRendererPatch : ModCorePatch
    {
        private static void PatchField(TypeDefinition liquidRenderer, AssemblyDefinition terraria)
        {
            var fields = liquidRenderer.Fields;
            ModifyField(fields.First(i => i.Name.Contains("WATERFALL_LENGTH")), FieldAttributes.Public | FieldAttributes.Static);
            ModifyField(fields.First(i => i.Name.Contains("DEFAULT_OPACITY")), FieldAttributes.Public | FieldAttributes.Static);
            ModifyField(fields.First(i => i.Name.Contains("WAVE_MASK_STRENGTH")), FieldAttributes.Public | FieldAttributes.Static);
            ModifyField(fields.First(i => i.Name.Contains("VISCOSITY_MASK")), FieldAttributes.Public | FieldAttributes.Static);
			ModifyField(fields.First(i => i.Name.Contains("_animationFrame")), FieldAttributes.Public | FieldAttributes.Static);
			ModifyField(fields.First(i => i.Name.Contains("_drawArea")), FieldAttributes.Public | FieldAttributes.Static);
			ModifyField(fields.First(i => i.Name.Contains("_drawCache")), FieldAttributes.Public | FieldAttributes.Static);
			ModifyField(fields.First(i => i.Name.Contains("_drawCacheForShimmer")), FieldAttributes.Public | FieldAttributes.Static);
		}

        private static void ModifyField(FieldDefinition field, Mono.Cecil.FieldAttributes attributes)
        {
            field.Publicize();
            field.Attributes &= ~FieldAttributes.InitOnly;
        }
    }
}
