using System;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace ReverieMod.Common.Systems
{
    public class LigneousTempleSystem : ModSystem
    {
        public class LigneousTemplePass : GenPass
        {
            public LigneousTemplePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Ligneous Temple";
                /* TODO:
                 * Calculate a square and a 'map' for said square, use as foundation for generation.
                 * procedurally generate "rooms" that spawn in the square map, make sure they dont place randomly and.
                 * apply the same method for tunnels, these connect rooms.
                 * create the boss room and treasure rooms, make them spawn at the bottom of the temple map.
                 * create the puzzle rooms that require wiring logic to pass. (disabling wiring for the player while in the temple may be necessary)
                 * (optional) create secret rooms that are hidden.
                 * additional things such as tile smoothing, pots, foliage/ambient tiles. decorative additions.
                */

            }
        }
    }
}
