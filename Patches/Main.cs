using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorePatcher;
using CorePatcher.Attributes;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using LiquidAPI.IO;

namespace LiquidAPI.Patches
{
    [PatchType("Terraria.Main")]
    // [PatchType(typeof(Terraria.Main))] // coming in the next core patcher update
    internal class MainPatch : ModCorePatch
    {
        private static void PatchLiquidType(TypeDefinition main, AssemblyDefinition terraria)
        {
            var drawWaters = main.Methods.FirstOrDefault(i => i.Name == "DrawWaters");

            drawWaters.Body.Instructions.Clear();
            drawWaters.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }
    }
}
