using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorePatcher;
using CorePatcher.Attributes;
using Mono.Cecil;

namespace LiquidAPI.Patches
{
    [PatchType("Terraria.SceneMetrics")]
    internal class SceneMetricsPatch : ModCorePatch
    {
        private static void PatchLiquidCount(TypeDefinition typeDefinition, AssemblyDefinition terraria)
        {
            var _liquidCounts = typeDefinition.Fields.First(i => i.Name == "_liquidCounts");
            _liquidCounts.Attributes &= ~FieldAttributes.InitOnly;
        }
    }
}
