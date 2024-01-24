using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using static EmpyreanDreamscape.ReverieSystem;

namespace EmpyreanDreamscape
{
    public class WorldGenerationSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ReverieIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Altars"));
            if (ReverieIndex != 1)
            {
                tasks.Insert(ReverieIndex + 1, new ReveriePass("Reverie Tree", 100f));
            }

            int ForestTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Wet Jungle"));
            if (ForestTempleIndex != 1)
            {
                tasks.Insert(ForestTempleIndex + 1, new ForestTemplePass("Reverie Temple", 100f));
            }
            int ReverieExtrasIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Smooth World"));
            if (ReverieExtrasIndex != 1)
            {
                tasks.Insert(ReverieExtrasIndex + 1, new ReverieExtrasPass("Reverie Extras", 100f));
            }
        }    
    }
}