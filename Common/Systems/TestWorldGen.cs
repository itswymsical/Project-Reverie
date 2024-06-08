using ReverieMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;


namespace ReverieMod.Common.Systems
{
    public class TestWorldGen : ReverieModSystem
    {
        public class TestPass : GenPass
        {
            public TestPass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Noise Generation";

                // Create and configure FastNoise object
                FastNoiseLite noise = new FastNoiseLite();
                noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
                noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
                noise.SetFractalGain(0.300f);
                noise.SetFractalOctaves(4);
                noise.SetFrequency(0.014f);

                int posx = Main.maxTilesX / 3;
                int posy = Main.maxTilesY / 3;
                float threshold = 0.1f; // Define your noise threshold
                // Gather noise data
                float[,] noiseData = new float[posx, posy];

                for (int x = 0; x < posx; x++)
                {
                    for (int y = 0; y < posy; y++)
                    {
                        noiseData[x, y] = noise.GetNoise(x, y);
                    }
                }

                for (int x = 0; x < posx; x++)
                {
                    for (int y = 0; y < posy; y++)
                    {
                        if (noiseData[x, y] < threshold)
                        {
                            int worldX = x + posx;
                            int worldY = y + posy;

                            // Break the tile at (worldX, worldY)
                            if (Main.tile[worldX, worldY].HasTile)
                            {
                                WorldGen.KillTile(worldX, worldY);
                            }
                        }
                        float progressPercentage = (float)((x * posy + y) + (posx * posy)) / (2 * posx * posy);
                        progress.Set(progressPercentage);
                    }
                }
            }
        }
    }
}
