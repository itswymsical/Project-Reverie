using Terraria.ModLoader;
using Terraria;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;
using ReverieMod.Content.Tiles;
using ReverieMod.Helpers;
using ReverieMod.Content.Tiles.WoodlandCanopy;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Metadata;

namespace ReverieMod.Common.Systems.Reverie
{
    public class TreeSystem : ModSystem
    {
        public class Reverie2Pass : GenPass
        {
            private int treeWood = TileID.LivingWood;
            private int treeLeaves = TileID.LeafBlock;
            private int canopyGrass = ModContent.TileType<WoodlandGrassTile>();
            public Reverie2Pass(string name, float loadWeight) : base(name, loadWeight)
            {
            }

            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Manifesting Reverie";

                int trunkX = GetTrunkX(out int trunkTopY, out int trunkBottomY);
                GenerateTrunk(trunkX, trunkTopY, trunkBottomY);

                int centerX = trunkX;
                int centerY = trunkBottomY + (Main.maxTilesY - trunkBottomY) / 4;
                GenerateBiome(centerX, centerY);
            }

            private int GetTrunkX(out int trunkTopY, out int trunkBottomY)
            {
                int trunkDir = Main.rand.Next(2);
                int spawnX = Main.maxTilesX / 2;
                int spawnY = (int)Main.worldSurface - (Main.maxTilesY / 12);
                int distance = (Main.maxTilesX - spawnX) / 20;
                int trunkX = trunkDir == 0 ? spawnX - distance : spawnX + distance;
                trunkX = Math.Clamp(trunkX, 0, Main.maxTilesX - 1); // Safety

                trunkTopY = (int)(spawnY - (Main.maxTilesY - spawnY) / 8);
                trunkBottomY = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 8);

                return trunkX;
            }

            private void GenerateTrunk(int trunkX, int trunkTopY, int trunkBottomY)
            {
                int trunkWidth = 10;
                const float curveFrequency = 0.0765f;
                const int curveAmplitude = 4;

                for (int j = trunkTopY; j <= trunkBottomY; j++)
                {
                    int currentTrunkWidth = trunkWidth + (j % 5 == 0 ? 2 : 0);
                    int curveOffset = (int)(Math.Sin(j * curveFrequency) * curveAmplitude);

                    int leftBound = trunkX - currentTrunkWidth / 2 + curveOffset;
                    int rightBound = trunkX + currentTrunkWidth / 2 + curveOffset;

                    for (int i = leftBound; i <= rightBound; i++)
                    {
                        WorldGen.KillWall(i, j);
                        WorldGen.PlaceTile(i, j, treeWood, forced: true);
                    }
                }

                for (int y = trunkTopY; y <= trunkBottomY; y++)
                {
                    int tunnelTrunkWidth = (trunkWidth / 2) + (y % 5 == 0 ? 2 : 0);
                    int tunnelOffset = (int)(Math.Sin(y * curveFrequency) * curveAmplitude);
                    int leftBound = trunkX - tunnelTrunkWidth / 2 + tunnelOffset;
                    int rightBound = trunkX + tunnelTrunkWidth / 2 + tunnelOffset;

                    for (int x = leftBound; x <= rightBound; x++)
                    {
                        WorldGen.KillTile(x, y);
                        WorldGen.PlaceWall(x, y, WallID.LivingWoodUnsafe);
                    }
                }

                GenerateLeaves(trunkX, trunkTopY, 4);
            }

            private void GenerateLeaves(int trunkX, int trunkY, int thickness)
            {
                float[] angles = new float[] { MathHelper.ToRadians(-120), MathHelper.ToRadians(-90), MathHelper.ToRadians(-45), MathHelper.ToRadians(45), MathHelper.ToRadians(90), MathHelper.ToRadians(120) };
                int branchLength = 32;

                foreach (float angle in angles)
                {
                    for (int j = 0; j < branchLength; j++)
                    {
                        float curve = (float)Math.Sin(j * 0.09f) * 3f;
                        int posX = trunkX + (int)(j * Math.Cos(angle + curve));
                        int posY = trunkY - (int)(j * Math.Sin(angle + curve));

                        for (int tx = -thickness; tx <= thickness; tx++)
                        {
                            for (int ty = -thickness; ty <= thickness; ty++)
                            {
                                if (tx * tx + ty * ty <= thickness * thickness)
                                {
                                    WorldGen.TileRunner(posX + tx, posY + ty, 30, 30, treeLeaves, true, 1, 1);
                                }
                            }
                        }
                    }
                }
            }

            private void GenerateBiome(int centerX, int centerY)
            {
                // Your biome generation code here
            }
        }
    }
}