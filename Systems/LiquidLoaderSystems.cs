using CorePatcher;
using LiquidAPI.APIs;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace LiquidAPI.Systems
{
    internal class LiquidLoaderSystems : ModSystem
    {
        internal static AssemblyDefinition LiquidAPIDef;

        public override void Load()
        {
            Main.versionNumber = "LiquidAPI v2 bitch";
            Main.versionNumber2 = "LiquidAPI v2 bitch";
            if (PatchLoader.DetectPatchedAssembly())
                LiquidLoader.ResizeArray();
            CorePatcher.PatchLoader.RegisterPrePatchOperation(() =>
            {
                SaveModDll(Mod);
                LiquidAPIDef = LoadPhysicalAssemblyFromModules("LiquidAPI.dll");
                PatchLoader.AddDeps(LiquidAPIDef);
            });
            
        }

        public override void Unload()
        {
            if (PatchLoader.DetectPatchedAssembly())
                LiquidLoader.ResizeArray(true);
        }

        private static void SaveModDll(Mod mod)
        {
            var module = mod.GetFileBytes(mod.Name + ".dll");
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "modules"));
            File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "modules", Path.GetFileName(mod.Name + ".dll")), module);
        }

        private static AssemblyDefinition LoadPhysicalAssemblyFromModules(string assemblyName)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "modules", assemblyName);
            return AssemblyDefinition.ReadAssembly(path, new ReaderParameters(ReadingMode.Immediate)
            {
                ReadWrite = true,
                InMemory = true
            });
        }

        public override void ClearWorld()
        {
            LiquidLoader.Liquids.Clear();
        }
    }
}
