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

namespace LiquidAPI.Patches
{
	[PatchType("Terraria.Main")]
	// [PatchType(typeof(Terraria.Main))] // coming in the next core patcher update
	internal class MainPatch : ModCorePatch
	{
		private static void RemoveLiquidRendering(TypeDefinition type, AssemblyDefinition terraria)
		{
			var method = type.Methods.FirstOrDefault(m => m.Name == "DrawWaters");

			ILCursor ilCursor = new ILCursor(new ILContext(method));
			method.Body.Instructions.Clear();
			ilCursor.EmitLdarg1();
			ilCursor.EmitDelegate((bool isBackground) =>
			{
				LiquidRendering.instance.InitateDrawLiquids(isBackground);
			});
			ilCursor.EmitRet();
		}
	}

}
