using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ID;

namespace Trelamium
{
    public class DruidaeaTreeSystem : ModSystem
    {
        public static bool EntropyMode;

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int DruidaeaIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Guide"));
            if (DruidaeaIndex != 1)
            {
                tasks.Insert(DruidaeaIndex + 1, new DruidaeaTrunkPass("Druidaea Tree", 100f));
            }
        }
        public class DruidaeaTrunkPass : GenPass
        {
            public DruidaeaTrunkPass(string name, float loadWeight) : base(name, loadWeight) {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Druidaea Tree";
                int x = Main.spawnTileX - 70;
                int y = (int)WorldGen.worldSurface;
                
                WorldGen.digTunnel(x, y, 0, 12, 60, 5);
            }
        }
    }
}