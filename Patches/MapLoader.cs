using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorePatcher.Attributes;
using CorePatcher.Utilities;
using Mono.Cecil;

namespace LiquidAPI.Patches
{
    [PatchType("Terraria.ModLoader.MapLoader")]
    internal class MapLoaderPatch
    {
        private static void Publicize(TypeDefinition typeDefinition, AssemblyDefinition terraria)
        { 
            typeDefinition.Fields.First(i => i.Name == "initialized").Publicize();
        }
    }
}
