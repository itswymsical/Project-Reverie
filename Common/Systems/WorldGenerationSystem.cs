using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using static ReverieMod.Common.Systems.ReverieTreeSystem;

namespace ReverieMod.Common.Systems
{
    public class WorldGenerationSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ReverieIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Stalac"));
            if (ReverieIndex != 1)
            {
                tasks.Insert(ReverieIndex + 1, new TrunkPass("Reverie Tree", 100f));
            }
            int ReverieExtrasIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Broken Traps"));
            if (ReverieExtrasIndex != 1)
            {
                tasks.Insert(ReverieExtrasIndex + 1, new ReverieExtrasPass("Reverie Extras", 74f));
            }
        }    
    }
}