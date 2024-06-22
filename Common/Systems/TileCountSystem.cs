using System;
using Terraria.ModLoader;
using ReverieMod.Content.Tiles.Canopy;
using Terraria.ID;

namespace ReverieMod.Common.Systems
{
    internal class TileCountSystem : ModSystem
    {
        public int canopyBlockCount;
        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            canopyBlockCount = tileCounts[TileID.LivingWood];
        }
    }
}
