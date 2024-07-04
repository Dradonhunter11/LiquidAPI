using LiquidAPI.APIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace LiquidAPI.Liquids
{
    public class UnloadedLiquid : ModLiquid
    {
        public override void SetStaticDefaults()
        {
            LiquidLoader.Liquids.unloadedTypes.Add(Type);
        }
    }
}
