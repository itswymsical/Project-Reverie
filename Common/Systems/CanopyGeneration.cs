using System;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace ReverieMod.Common.Systems
{
    public class CanopyGeneration : ModSystem
    {
        public class CanopyPass : GenPass
        {
            public CanopyPass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Woodland Canopy";
                /* TODO:
                 * Generate a natural, non-uniform shape, use as foundation for generation.
                 * procedurally generate caverns with perlin noise or cellular automata.
                 * Generate ovualar/dome shaped spaces that have lots of ambient tiles. Generate shrine structures.
                 * 
                 * create the puzzle rooms that require wiring logic to pass. (disabling wiring for the player while in the temple may be necessary)
                 * (optional) create secret rooms that are hidden.
                 * additional things such as tile smoothing, pots, foliage/ambient tiles. decorative additions.
                */

            }
        }
    }
}