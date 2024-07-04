using CorePatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorePatcher.Attributes;
using Mono.Cecil;
using CorePatcher.Utilities;
using LiquidAPI.APIs;
using Terraria.ModLoader.IO;
using Mono.Cecil.Cil;
using System.Security.Cryptography.Xml;
using Mono.Cecil.Rocks;
using MonoMod.Utils;

namespace LiquidAPI.Patches
{
   
    internal class TileIOPatch
    {
        [PatchType("Terraria.ModLoader.IO.TileIO")]
        public class TileIOPublicize : ModCorePatch
        {
            private static void Publicize(TypeDefinition type, AssemblyDefinition terraria)
            {
                type.Publicize();

                foreach(var nestedType in type.NestedTypes) {
                    /*nestedType.Publicize();
                    foreach(var field in nestedType.Fields) {
                        field.Publicize();
                    }
                    foreach(var method in nestedType.Methods) {
                        method.Publicize();
                    }*/
                }
            }
        }

        [PatchType("Terraria.ModLoader.IO.TileIO")]
        public class TileIOPatches : ModCorePatch
        {
            private static void PatchResetUnloadedType(TypeDefinition type, AssemblyDefinition terraria)
            {
                var SaveBasics = type.Methods.First(i => i.Name == "ResetUnloadedTypes");

                var ioImplType = new GenericInstanceType(type.Module.ImportReference(typeof(Terraria.ModLoader.IO.TileIO.IOImpl<,>)));
                ioImplType.GenericArguments.Add(type.Module.ImportReference(typeof(ModLiquid)));
                ioImplType.GenericArguments.Add(type.Module.ImportReference(typeof(LiquidEntry)));

                var unloadType = ioImplType.Resolve().Fields.First(i => i.Name == "unloadedTypes");
                var Save = ioImplType.Resolve().Methods.First(i => i.Name == "Save");

                var a = new FieldReference(unloadType.Name, type.Module.ImportReference(unloadType.FieldType), ioImplType);

                var instructionList = SaveBasics.Body.GetILProcessor().Body.Instructions;

                List<Instruction> instructions = new List<Instruction>();
                instructions.Add(Instruction.Create(OpCodes.Callvirt, type.Module.ImportReference((MethodReference)instructionList[2].Operand)));
                instructions.Add(Instruction.Create(OpCodes.Ldfld, type.Module.ImportReference(a)));
                instructions.Add(Instruction.Create(OpCodes.Ldsfld, type.Module.ImportReference(LiquidLoader.GetLiquidsReference())));

                int index = instructionList.Count - 1;
                foreach (Instruction instruction in instructions)
                {
                    instructionList.Insert(index, instruction);
                }
            }

            private static void PatchSaveBasic(TypeDefinition type, AssemblyDefinition terraria)
            {
                var SaveBasics = type.Methods.First(i => i.Name == "SaveBasics");

                var ioImplType = new GenericInstanceType(type.Module.ImportReference(typeof(Terraria.ModLoader.IO.TileIO.IOImpl<,>)));
                ioImplType.GenericArguments.Add(type.Module.ImportReference(typeof(ModLiquid)));
                ioImplType.GenericArguments.Add(type.Module.ImportReference(typeof(LiquidEntry)));

                var Save = ioImplType.Resolve().Methods.First(i => i.Name == "Save");

                var a = new MethodReference(Save.Name, Save.ReturnType, ioImplType)
                {
                    HasThis = true
                };
                a.Parameters.Add(new ParameterDefinition(type.Module.ImportReference(typeof(Terraria.ModLoader.IO.TagCompound))));

                var instructionList = SaveBasics.Body.GetILProcessor().Body.Instructions;

                List<Instruction> instructions = new List<Instruction>();
                instructions.Add(Instruction.Create(OpCodes.Callvirt, a));
                instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
                instructions.Add(Instruction.Create(OpCodes.Ldsfld, type.Module.ImportReference(LiquidLoader.GetLiquidsReference())));

                int index = instructionList.Count - 2;
                foreach (Instruction instruction in instructions)
                {
                    instructionList.Insert(index, instruction);
                }
            }

            /*
                    IL_001a: ldsfld class LiquidAPI.APIs.LiquidIOImpl LiquidAPI.APIs.LiquidLoader::Liquids
                    IL_0001: ldsfld class LiquidAPI.APIs.LiquidIOImpl LiquidAPI.APIs.LiquidLoader::Liquids
	                IL_0006: callvirt instance void class Terraria.ModLoader.IO.TileIO/IOImpl`2<class LiquidAPI.APIs.ModLiquid, class LiquidAPI.APIs.LiquidEntry>::Clear()
                    IL_000f: callvirt instance void class Terraria.ModLoader.IO.TileIO/IOImpl`2<class Terraria.ModLoader.ModWall, class Terraria.ModLoader.IO.WallEntry>::Clear()
             */

            private static void PatchClearWorld(TypeDefinition type, AssemblyDefinition terraria)
            {
                var ClearWorld = type.Methods.First(i => i.Name == "ClearWorld");

                var ioImplType = new GenericInstanceType(type.Module.ImportReference(typeof(Terraria.ModLoader.IO.TileIO.IOImpl<,>)));
                ioImplType.GenericArguments.Add(type.Module.ImportReference(typeof(ModLiquid)));
                ioImplType.GenericArguments.Add(type.Module.ImportReference(typeof(LiquidEntry)));

                var Clear = ioImplType.Resolve().Methods.First(i => i.Name == "Clear");

                var a = new MethodReference(Clear.Name, Clear.ReturnType, ioImplType)
                {
                    HasThis = true
                };

                var instructionList = ClearWorld.Body.GetILProcessor().Body.Instructions;

                List<Instruction> instructions = new List<Instruction>();
                instructions.Add(Instruction.Create(OpCodes.Callvirt, a));
                instructions.Add(Instruction.Create(OpCodes.Ldsfld, type.Module.ImportReference(LiquidLoader.GetLiquidsReference())));


                int index = instructionList.Count - 1;
                foreach (Instruction instruction in instructions)
                {
                    instructionList.Insert(index, instruction);
                }
            }

            private static void PatchLoadBasics(TypeDefinition type, AssemblyDefinition terraria)
            {
                var loadBasics = type.Methods.First(i => i.Name == "LoadBasics");

                loadBasics.Body.Variables.Add(new Mono.Cecil.Cil.VariableDefinition(type.Module.ImportReference(LiquidLoader.GetliquidEntriesTypeReference())));
                var liquidEntry = type.Module.ImportReference(LiquidEntry.GetTypeDefinition());

                var ioImplType = new GenericInstanceType(type.Module.ImportReference(typeof(Terraria.ModLoader.IO.TileIO.IOImpl<,>)));
                ioImplType.GenericArguments.Add(type.Module.ImportReference(typeof(ModLiquid)));
                ioImplType.GenericArguments.Add(type.Module.ImportReference(typeof(LiquidEntry)));

                var LoadEntries = ioImplType.Resolve().Methods.First(i => i.Name == "LoadEntries");
                
                var a = new MethodReference(LoadEntries.Name, LoadEntries.ReturnType, ioImplType);

                var instructionList = loadBasics.Body.GetILProcessor().Body.Instructions;
                a.Parameters.Add((instructionList[3].Operand as MethodReference).Parameters[1]);

                List<Instruction> instructions = new List<Instruction>();
                instructions.Add(Instruction.Create(OpCodes.Callvirt, new MethodReference("LoadEntries", type.Module.TypeSystem.Void, ioImplType)
                {
                    HasThis = true,
                    Parameters =
                    {
                        new ParameterDefinition(type.Module.ImportReference(typeof(Terraria.ModLoader.IO.TagCompound))),
                        new ParameterDefinition("TEntry", ParameterAttributes.Out, (instructionList[3].Operand as MethodReference).Parameters[1].ParameterType)
                    }
                }));
                instructions.Add(Instruction.Create(OpCodes.Ldloca_S, loadBasics.Body.Variables[2]));
                instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                instructions.Add(Instruction.Create(OpCodes.Ldsfld, type.Module.ImportReference(LiquidLoader.GetLiquidsReference())));

                foreach(Instruction instruction in instructions) {
                    instructionList.Insert(8, instruction);
                }

                instructions.Clear();
                instructions.Add(Instruction.Create(OpCodes.Callvirt, new MethodReference("LoadData", type.Module.TypeSystem.Void, ioImplType)
                {
                    HasThis = true,
                    Parameters =
                    {
                        new ParameterDefinition(type.Module.ImportReference(typeof(Terraria.ModLoader.IO.TagCompound))),
                        new ParameterDefinition("TEntry", ParameterAttributes.None, (instructionList[instructionList.Count - 3].Operand as MethodReference).Parameters[1].ParameterType)
                    }
                }));
                instructions.Add(Instruction.Create(OpCodes.Ldloc_2));
                instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                instructions.Add(Instruction.Create(OpCodes.Ldsfld, type.Module.ImportReference(LiquidLoader.GetLiquidsReference())));

                foreach (Instruction instruction in instructions)
                {
                    instructionList.Insert(12, instruction);
                }
            }

            private static GenericInstanceMethod GenerateGenericMethod(MethodDefinition tile, List<TypeReference> typeReferences)
            {
                
                var b = tile.Resolve();
                var c = new GenericInstanceMethod(b);
                foreach(TypeReference type in typeReferences) {
                    c.GenericArguments.Add(type);
                }
                
                return c;
            }

            private static GenericInstanceType GenerateGenericType(TypeDefinition tile, List<TypeReference> typeReferences)
            {
                var b = tile.Resolve();
                var c = new GenericInstanceType(b);
                foreach (TypeReference type in typeReferences)
                {
                    c.GenericArguments.Add(type);
                }
                return c;
            }
        }
    }
}
