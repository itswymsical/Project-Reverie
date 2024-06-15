using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using static ReverieMod.Common.Systems.CanopyWorldGen;
using static ReverieMod.Common.Systems.TestWorldGen;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.Biomes;

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

            int CanopyIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Stalac"));
            if (CanopyIndex != 1)
            {
                tasks.Insert(CanopyIndex + 1, new CanopyPass("Woodland Canopy", 100f));
                tasks.Insert(CanopyIndex + 2, new CanopyRootPass("Canopy Cave System", 100f));
                tasks.Insert(CanopyIndex + 3, new ReverieTreePass("Reverie Extras", 74f));
            }
        }    
    }
}