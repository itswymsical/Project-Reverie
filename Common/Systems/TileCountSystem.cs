using System;
using Terraria.ModLoader;
using EmpyreanDreamscape.Content.Tiles.DruidsGarden;

namespace EmpyreanDreamscape.Common.Systems
{
    internal class TileCountSystem :ModSystem
    {
        public int druidsBlockCount;
        public int quagmireBlockCount;
        public int shroomBlockCount;
        public int ruinsBlockCount;
        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            druidsBlockCount = tileCounts[ModContent.TileType<LoamTileGrass>()];
            ruinsBlockCount = tileCounts[ModContent.TileType<SlateTile>()];
        }
    }
}
