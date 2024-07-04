using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorePatcher;
using CorePatcher.Attributes;
using CorePatcher.Utilities;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace LiquidAPI.Patches
{
    [PatchType("Terraria.ModLoader.MapEntry")]
    internal class MapEntryPatch : ModCorePatch
    {
        private static void Publicize(TypeDefinition type, AssemblyDefinition terraria)
        {
            type.Publicize();
            foreach (var cctor in type.GetConstructors()) {
                cctor.Publicize();
            }
        }
    }
}
