using ReLogic.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace LiquidAPI.APIs
{
    public class ModLiquidID
    {
        public const short Water = LiquidID.Water;
        public const short Lava = LiquidID.Lava;
        public const short Honey = LiquidID.Honey;
        public const short Shimmer = LiquidID.Shimmer;
        public static readonly short Count = 4;
        public static readonly IdDictionary Search = IdDictionary.Create<ModLiquidID, ushort>();

    }
}
