using LiquidAPI.Systems;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace LiquidAPI.APIs
{
    public static class LiquidLoader
    {
        public static readonly IDictionary<ushort, IList<MapEntry>> liquidEntries = new Dictionary<ushort, IList<MapEntry>>();
        public static readonly IDictionary<ushort, ushort> entryToLiquid = new Dictionary<ushort, ushort>();

        public static LiquidIOImpl Liquids = new LiquidIOImpl();

        private static int nextLiquid = LiquidID.Count;
        private static bool loaded = false;
        public static int LiquidCount => nextLiquid;

        public static readonly IList<ModLiquid> liquids = new List<ModLiquid>();

        public static ModLiquid GetLiquid(int type)
        {
            return type >= LiquidCount && type < LiquidCount ? liquids[type - LiquidID.Count] : null;
        }

        internal static int ReserveLiquidID()
        {
            if(ModNet.AllowVanillaClients)
                throw new Exception("Adding liquid breaks vanilla client compatibility");
            int reserveId = nextLiquid;
            nextLiquid++; 
            return reserveId;
        }

        internal static void ResizeArray(bool unloading = false)
        {
            LoaderUtils.ResetStaticMembers(typeof(ModLiquidID));

            Resize<LiquidRenderer, int>("WATERFALL_LENGTH", nextLiquid, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            Resize<LiquidRenderer, float>("DEFAULT_OPACITY", nextLiquid, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            Resize<LiquidRenderer, byte>("WAVE_MASK_STRENGTH", nextLiquid + 1, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            Resize<LiquidRenderer, byte>("VISCOSITY_MASK", nextLiquid + 1, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            Resize<SceneMetrics, int>("_liquidCounts", nextLiquid, BindingFlags.NonPublic | BindingFlags.Instance, Main.SceneMetrics);
            // Resize<SceneMetrics, int>("_liquidCounts", nextLiquid, BindingFlags.NonPublic | BindingFlags.Instance, Main.PylonSystem._s);

            LiquidRenderer.WATERFALL_LENGTH[1] = 2;

            setAValue<float>("DEFAULT_OPACITY", 0.3f);

			Array.Resize(ref LiquidTextureAssets.Liquid, nextLiquid);

			if (!unloading)
            {
                loaded = true;
            }
        }

        internal static void Resize<Tinput, Ttype>(string fieldName, int newValue, BindingFlags bindingFlags, object instance = null)
        {
            var field = typeof(Tinput).GetField(fieldName, bindingFlags);
            Ttype[] value = (Ttype[])field.GetValue(instance);
            Array.Resize(ref value, newValue);
            field.SetValue(instance, value); 
        }

        internal static void setAValue<T>(string fieldName, T newValue)
        {
            var field = typeof(LiquidRenderer).GetField(fieldName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            T[] value = (T[])field.GetValue(null);
            value[1] = newValue;
            field.SetValue(null, value);
        }

        internal static TypeReference GetTypeReference()
        {
            return LiquidLoaderSystems.LiquidAPIDef.MainModule.Types.First(i => i.FullName == "LiquidAPI.APIs.LiquidLoader");
        }

        /// <summary>
        /// Too lazy to make a type reference from scratch, especially an array
        /// </summary>
        private static LiquidEntry[] liquidEntriesReference;

        internal static TypeReference GetliquidEntriesTypeReference()
        {
            var a = GetTypeReference().Resolve();
            return a.Fields.First(i => i.Name == "liquidEntriesReference").FieldType;
        }

        internal static FieldReference GetLiquidsReference()
        {
            var a = GetTypeReference().Resolve();
            return a.Fields.First(i => i.Name == "Liquids");
        }
    }
}
