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
    [PatchType("Terraria.Tile")]
    internal class TilePatch : ModCorePatch
    {
        private static void PatchLiquidType(TypeDefinition tile, AssemblyDefinition terraria)
        {
            PatchGetProperty(tile, tile.Properties.First(i => i.Name == "LiquidType").GetMethod, TileLiquid.GetLiquidType());
            PatchSetProperty(tile, tile.Properties.First(i => i.Name == "LiquidType").SetMethod, TileLiquid.SetLiquidType());
            PatchGetProperty(tile, tile.Properties.First(i => i.Name == "SkipLiquid").GetMethod, TileLiquid.GetSkipLiquid());
            PatchSetProperty(tile, tile.Properties.First(i => i.Name == "SkipLiquid").SetMethod, TileLiquid.SetSkipLiquid());
            PatchGetProperty(tile, tile.Properties.First(i => i.Name == "CheckingLiquid").GetMethod, TileLiquid.GetCheckingLiquid());
            PatchSetProperty(tile, tile.Properties.First(i => i.Name == "CheckingLiquid").SetMethod, TileLiquid.SetCheckingLiquid());
        }

        private static void PatchGetProperty(TypeDefinition tile, MethodDefinition a, MethodReference reference)
        {
            a.Body.GetILProcessor().Clear();
            ILContext context = new ILContext(a);
            ILCursor cursor = new ILCursor(context);
            cursor.EmitLdarg0();
            cursor.EmitCall(GenerateGenericMethod(tile, cursor.Module.ImportReference(TileLiquid.GetTypeReference())));
            cursor.EmitCall(cursor.Module.ImportReference(reference));
            cursor.EmitRet();
        }

        private static void PatchSetProperty(TypeDefinition tile, MethodDefinition a, MethodReference reference)
        {
            a.Body.GetILProcessor().Clear();
            ILContext context = new ILContext(a);
            ILCursor cursor = new ILCursor(context);
            cursor.EmitLdarg0();
            cursor.EmitCall(GenerateGenericMethod(tile, cursor.Module.ImportReference(TileLiquid.GetTypeReference())));
            cursor.EmitLdarg1();
            cursor.EmitCall(cursor.Module.ImportReference(reference));
            cursor.EmitRet();
        }

        private static GenericInstanceMethod GenerateGenericMethod(TypeDefinition tile, TypeReference reference)
        {
            var getDef = tile.Methods.First(i => i.Name == "Get" || i.IsGenericInstance);
            if(getDef == null)
            {
                throw new Exception("failure");
            }
            var b = getDef.Resolve();
            var c = new GenericInstanceMethod(b);
            c.GenericArguments.Add(reference);
            c.GenericArguments[0].IsValueType = true;
            return c;
        }
    }
}
