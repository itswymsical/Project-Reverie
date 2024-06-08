﻿using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using static ReverieMod.Common.Systems.CanopyWorldGen;
using static ReverieMod.Common.Systems.TestWorldGen;

namespace ReverieMod.Common.Systems
{
    public class WorldGenerationSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            /*
            int NoiseIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Settle Liquids"));
            if (NoiseIndex != 1)
            {
                tasks.Insert(NoiseIndex + 1, new TestPass("Noise Test", 100f));
            }*/

            
            int CanopyIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Settle Liquids"));
            if (CanopyIndex != 1)
            {
                tasks.Insert(CanopyIndex + 1, new CanopyPass("Woodland Canopy", 100f));
            }
            int ReverieExtrasIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Smooth World"));
            if (ReverieExtrasIndex != 1)
            {
                tasks.Insert(ReverieExtrasIndex + 1, new ReverieTreePass("Reverie Extras", 74f));
            }
        }    
    }
}