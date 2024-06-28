using ReverieMod.Helpers;
using ReverieMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;


namespace ReverieMod.Common.Systems.Subworlds.Archaea
{
    public class DesertPass : GenPass
    {
        public DesertPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating Terrain";
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(2);
            noise.SetFrequency(0.01f);

            Main.spawnTileX = Main.maxTilesX / 2;
            Main.spawnTileY = (int)Main.worldSurface - 10;
            // Full Desert
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                // Get noise value for current x position.
                float noiseValue = noise.GetNoise(x, Main.maxTilesY / 2);

                // Scale noise value to fit within terrain height range starting from worldSurface.
                int height = (int)(Main.worldSurface - 10 + noiseValue * 25); // Adjust 20 for more or less height variation.

                // Ensure height is within valid range.
                height = Math.Clamp(height, 0, Main.maxTilesY - 1);

                // Generate terrain based on noise value.
                for (int y = height; y < Main.UnderworldLayer; y++)
                {
                    // Place sandstone tiles below the sand to prevent it from falling
                    WorldGen.KillTile(x, y, noItem: true);
                    WorldGen.PlaceTile(x, y, TileID.Sandstone, true, true);
                    WorldGen.PlaceWall(x, y, WallID.SandstoneEcho);
                }
                for (int y = height; y < (int)Main.worldSurface + Main.worldSurface / 4; y++)
                {
                    WorldGen.PlaceTile(x, y, TileID.Sand, forced: true);
                }
            }
            Helper.SmoothTerrain(Main.maxTilesX, Main.maxTilesY);
        }
    }
}