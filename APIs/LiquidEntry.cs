using LiquidAPI.Liquids;
using LiquidAPI.Systems;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LiquidAPI.APIs
{

    public class LiquidIOImpl : TileIO.IOImpl<ModLiquid, LiquidEntry>
    {
        public LiquidIOImpl() : base("liquidMap", "liquidData")
        {
        }

        protected override int LoadedBlockCount => LiquidLoader.liquids.Count;

        protected override IEnumerable<ModLiquid> LoadedBlocks => LiquidLoader.liquids;

        protected override LiquidEntry ConvertBlockToEntry(ModLiquid liquid) => new LiquidEntry(liquid);

        protected override ushort GetModBlockType(Tile tile) => tile.LiquidType >= ModLiquidID.Count ? (ushort)tile.LiquidType : (ushort)0;

        protected override void ReadData(Tile tile, LiquidEntry entry, BinaryReader reader)
        {
            tile.LiquidType = entry.loadedType;
            tile.LiquidAmount = reader.ReadByte();
        }

        protected override void WriteData(BinaryWriter writer, Tile tile, LiquidEntry entry)
        {
            writer.Write(entry.loadedType);
            writer.Write(tile.LiquidAmount);
        }

        public static TypeDefinition GetTypeDefinition()
        {
            return LiquidLoaderSystems.LiquidAPIDef.MainModule.Types.First(i => i.FullName == "LiquidAPI.APIs.LiquidIOImpl");
        }

        public static MethodDefinition GetLoadEntries()
        {
            var a = GetTypeDefinition().BaseType.Resolve();
            return a.Methods.First(i => i.Name == "LoadEntries");
        }
    }

    public class LiquidEntry : ModEntry
    {
        public static Func<TagCompound, LiquidEntry> DESERIALIZER = tag => new LiquidEntry(tag);

        public LiquidEntry(ModBlockType block) : base(block)
        {
        }

        public LiquidEntry(TagCompound tag) : base(tag)
        {
        }

        public override string DefaultUnloadedType => ModContent.GetInstance<UnloadedLiquid>().FullName;

        protected override string GetUnloadedType(ushort type) => DefaultUnloadedType;

        public static TypeDefinition GetTypeDefinition()
        {
            return LiquidLoaderSystems.LiquidAPIDef.MainModule.Types.First(i => i.FullName == "LiquidAPI.APIs.LiquidEntry");
        }
    }
}
