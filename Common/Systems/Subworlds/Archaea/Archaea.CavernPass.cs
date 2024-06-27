using ReverieMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace ReverieMod.Common.Systems.Subworlds.Archaea
{
    public class CavernPass : GenPass
    {
        public CavernPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating caves";
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(5);
            noise.SetFrequency(0.085f);
            float threshold = 0.1f; // This is basically which noise values are we going to ignore/kill.
            int posx = Main.maxTilesX;
            int posy = Main.UnderworldLayer;
            float[,] noiseData = new float[posx, posy];

            for (int x = 0; x < posx; x++)
            {
                for (int y = 0; y < posy; y++)
                {
                    int worldX = x;
                    int worldY = y;

                    noiseData[x, y] = noise.GetNoise(worldX, worldY);
                }
            }
            for (int x = 0; x < posx; x++)
            {
                for (int y = (int)Main.rockLayer; y < posy; y++)
                {
                    int worldX = x;
                    int worldY = y;
                    if (noiseData[x, y] > -1f && noiseData[x, y] < -0.09f)
                        WorldGen.PlaceTile(worldX, worldY, TileID.HardenedSand, forced: true);

                    if (noiseData[x, y] > threshold)
                        WorldGen.KillTile(worldX, worldY);

                    progress.Set((float)(x * posy + y + posx * posy) / (2 * posx * posy));
                }
            }
        }
    }
}