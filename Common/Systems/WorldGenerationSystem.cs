using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using static ReverieMod.Common.Systems.CanopyWorldGen;

namespace ReverieMod.Common.Systems
{
    public class WorldGenerationSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
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