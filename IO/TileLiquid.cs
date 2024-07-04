using LiquidAPI.Systems;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace LiquidAPI.IO
{
    public struct TileLiquid : ITileData
    {
        public byte Amount;

        // c = checking liquid
        // s = skip liquid
        // l = liquid id
        private ushort typeAndFlags;

        public int LiquidType { get => TileDataPacking.Unpack(typeAndFlags, 0, 13); set => typeAndFlags = (byte)TileDataPacking.Pack(value, typeAndFlags, 0, 10); }
        public bool SkipLiquid { get => TileDataPacking.GetBit(typeAndFlags, 14); set => typeAndFlags = (byte)TileDataPacking.SetBit(value, typeAndFlags, 11); }
        public bool CheckingLiquid { get => TileDataPacking.GetBit(typeAndFlags, 15); set => typeAndFlags = (byte)TileDataPacking.SetBit(value, typeAndFlags, 12); }

        internal static MethodReference GetLiquidType()
        {
            TypeDefinition typeDefinition = GetTypeReference().Resolve();
            var liquidType = typeDefinition.Properties.First(i => i.Name == "LiquidType").GetMethod;
            return liquidType.Resolve();
        }

        internal static MethodReference SetLiquidType()
        {
            TypeDefinition typeDefinition = GetTypeReference().Resolve();
            var liquidType = typeDefinition.Properties.First(i => i.Name == "LiquidType").SetMethod;
            return liquidType.Resolve();
        }

        internal static MethodReference GetSkipLiquid()
        {
            TypeDefinition typeDefinition = GetTypeReference().Resolve();
            var liquidType = typeDefinition.Properties.First(i => i.Name == "SkipLiquid").GetMethod;
            return liquidType.Resolve();
        }

        internal static MethodReference SetSkipLiquid()
        {
            TypeDefinition typeDefinition = GetTypeReference().Resolve();
            var liquidType = typeDefinition.Properties.First(i => i.Name == "SkipLiquid").SetMethod;
            return liquidType.Resolve();
        }

        internal static MethodReference GetCheckingLiquid()
        {
            TypeDefinition typeDefinition = GetTypeReference().Resolve();
            var liquidType = typeDefinition.Properties.First(i => i.Name == "CheckingLiquid").GetMethod;
            return liquidType.Resolve();
        }

        internal static MethodReference SetCheckingLiquid()
        {
            TypeDefinition typeDefinition = GetTypeReference().Resolve();
            var liquidType = typeDefinition.Properties.First(i => i.Name == "CheckingLiquid").SetMethod;
            return liquidType.Resolve();
        }

        internal static TypeReference GetTypeReference() {
            if(_cache == null)
                _cache = LiquidLoaderSystems.LiquidAPIDef.MainModule.Types.First(i => i.FullName == "LiquidAPI.IO.TileLiquid");
            return _cache;
        }

        private static TypeReference _cache;
    }
}
