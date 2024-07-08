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
using LiquidAPI.Systems;
using System.Reflection;

namespace LiquidAPI.Patches
{
	[PatchType("Terraria.Main")]
	// [PatchType(typeof(Terraria.Main))] // coming in the next core patcher update
	public class MainPatch : ModCorePatch
	{
		private static void RemoveLiquidRendering(TypeDefinition type, AssemblyDefinition terraria)
		{
			var method = type.Methods.FirstOrDefault(m => m.Name == "DrawWaters");

			method.Body.Instructions.Clear();
			ILCursor ilCursor = new ILCursor(new ILContext(method));
			OpCode opCode;
			OpCode opCode2;

			ilCursor.EmitLdsfld(typeof(LiquidRendering).GetRuntimeField("instance"));
			ilCursor.EmitLdnull();
			ilCursor.EmitCeq();
			ilCursor.EmitStloc0();

			ilCursor.EmitLdloc0();
			ilCursor.EmitBrfalse(Instruction.Create(opCode));

			ilCursor.EmitNop();
			ilCursor.EmitBr(Instruction.Create(opCode2));

			//ilCursor.MarkLabel(lable);
			ilCursor.EmitLdsfld(typeof(LiquidRendering).GetRuntimeField("instance"));
			ilCursor.EmitLdarg1();
			ilCursor.EmitCallvirt(typeof(LiquidRendering).GetRuntimeMethod("InitateDrawLiquids", new Type[] { typeof(bool) }));

			ilCursor.EmitNop();
			ilCursor.EmitBr(Instruction.Create(opCode2));

			//ilCursor.MarkLabel(lable2);
			ilCursor.EmitRet();

			/*
			IL_0000: nop
			// if (LiquidRendering.instance != null)
			IL_0001: ldsfld class LiquidAPI.Systems.LiquidRendering LiquidAPI.Systems.LiquidRendering::'instance'
			IL_0006: ldnull
			IL_0007: ceq
			IL_0009: stloc.0
			// (no C# code)
			IL_000a: ldloc.0
			IL_000b: brfalse.s IL_0010

			// }
			IL_000d: nop
			IL_000e: br.s IL_001e

			// LiquidRendering.instance.InitateDrawLiquids(isBackground);
			IL_0010: ldsfld class LiquidAPI.Systems.LiquidRendering LiquidAPI.Systems.LiquidRendering::'instance'
			IL_0015: ldarg.1
			IL_0016: callvirt instance void LiquidAPI.Systems.LiquidRendering::InitateDrawLiquids(bool)
			// (no C# code)
			IL_001b: nop
			IL_001c: br.s IL_001e

			IL_001e: ret
			*/
		}

		public void DrawWaters(bool isBackground = true)
		{
            if (LiquidRendering.instance == null)
            {
				return;
            }
            LiquidRendering.instance.InitateDrawLiquids(isBackground);
			return;
			if (isBackground)
			{
				int i = 0;
			}


		}
	}

}
