using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Trelamium.Content.Tiles.DruidsGarden;
using System.ComponentModel;

namespace Trelamium
{
    public class DruidaeaTreeSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int DruidaeaIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Stalac"));
            if (DruidaeaIndex != 1)
            {
                tasks.Insert(DruidaeaIndex + 1, new DruidaeaTreePass("Druidaea Tree", 100f));
            }
        }

        public class DruidaeaTreePass : GenPass
        {
            public DruidaeaTreePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }

            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Planting a Druidaea seed";
                int trunkX;
                int trunkDir = Main.rand.Next(2);
                int distance = (Main.maxTilesX - Main.spawnTileX) / 24;
                if (trunkDir == 0)
                {
                    trunkX = Main.spawnTileX - distance;
                }
                else
                {
                    trunkX = Main.spawnTileX + distance;
                }
                trunkX = Math.Clamp(trunkX, 0, Main.maxTilesX - 1); //safety

                int trunkTopY = (int)(Main.worldSurface - (Main.maxTilesY - Main.worldSurface) / 6);
                int trunkBottomY = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 5);
                int trunkWidth = 38;

                const float curveFrequency = 0.1f;
                const int curveAmplitude = 5;

                for (int y = trunkTopY; y <= trunkBottomY; y++)
                {
                    // Calculate the current width and curve offset for this level of the trunk
                    int currentTrunkWidth = trunkWidth + (y % 5 == 0 ? 2 : 0); // Example: add extra width every 5 blocks
                    int curveOffset = (int)(Math.Sin(y * curveFrequency) * curveAmplitude); // Calculate the curve offset

                    // Calculate the left and right bounds of the trunk at this level
                    int leftBound = trunkX - currentTrunkWidth / 2 + curveOffset;
                    int rightBound = trunkX + currentTrunkWidth / 2 + curveOffset;

                    for (int x = leftBound; x <= rightBound; x++)
                    {
                        WorldGen.PlaceTile(x, y, TileID.LivingWood, forced: true);
                    }
                }
                GenerateBranches(trunkX + (Main.rand.Next(-5, 6)), trunkTopY, 50, 2);

                int centerX = trunkX;
                int centerY = trunkBottomY;
                int horizontalRadius = 238;
                int verticalRadius = 265;
                for (int x = centerX - horizontalRadius; x <= centerX + horizontalRadius; x++)
                {
                    for (int y = centerY - verticalRadius; y <= centerY + verticalRadius; y++)
                    {
                        if (((float)(x - centerX) * (x - centerX)) / (horizontalRadius * horizontalRadius) +
                            ((float)(y - centerY) * (y - centerY)) / (verticalRadius * verticalRadius) <= 1)
                        {
                            WorldGen.PlaceTile(x, y, ModContent.TileType<LoamTile>(), forced: true);
                            WorldGen.KillWall(x, y, false);
                        }
                    }
                }
                GenerateCellularAutomataCaves(centerX, centerY, horizontalRadius, verticalRadius);
                /*
                int numTunnels = Main.rand.Next(88, 171);
                for (int i = 0; i < numTunnels; i++)
                {
                    int randomX = Main.rand.Next(centerX - horizontalRadius, centerX + horizontalRadius);
                    int randomY = Main.rand.Next(centerY - verticalRadius, centerY + verticalRadius);

                    if (((float)(randomX - centerX) * (randomX - centerX)) / (horizontalRadius * horizontalRadius) + ((float)(randomY - centerY) * (randomY - centerY)) / (verticalRadius * verticalRadius) <= 1)
                    {
                        int tunnelWidth = Main.rand.Next(13, 27);
                        int tunnelLength = Main.rand.Next(19, 41);
                        int maxDirectionChange = 3;
                        WorldGen.digTunnel(randomX, randomY, 2.025f, 1.074573f, Main.rand.Next(11, 47), Main.rand.Next(3, 9));
                        WorldGen.CaveOpenater(randomX, randomY);
                        GardenTunnel(randomX, randomY, tunnelWidth, maxDirectionChange, randomY + tunnelLength);
                    }
                }
                */

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, trunkX, trunkTopY, trunkWidth, trunkBottomY - trunkTopY + 1);
                }
            }
            public void GenerateBranches(int trunkX, int trunkY, int numBranches, int thickness)
            {
                // Define the specific angles at which you want branches to grow
                float[] angles = new float[] { MathHelper.ToRadians(-120), MathHelper.ToRadians(-90), MathHelper.ToRadians(-45), MathHelper.ToRadians(45), MathHelper.ToRadians(90), MathHelper.ToRadians(120) };

                // Ensure you have the right number of branches for the angles defined
                numBranches = angles.Length;

                for (int i = 0; i < numBranches; i++)
                {
                    float angle = angles[i]; // Use the predefined angle
                    int branchLength = 56; // Random length for each branch

                    // Apply the curves as we did previously, with a sine wave function
                    for (int j = 0; j < branchLength; j++)
                    {
                        // Apply a gentle curvature using a sine wave
                        float curve = (float)Math.Sin(j * 0.07f) * 3f; // Adjust the frequency and amplitude as needed

                        // Calculate the position of the current segment
                        int posX = trunkX + (int)(j * Math.Cos(angle + curve));
                        int posY = trunkY - (int)(j * Math.Sin(angle + curve));

                        // Place tiles to make the branch thicker
                        for (int tx = -thickness; tx <= thickness; tx++)
                        {
                            for (int ty = -thickness; ty <= thickness; ty++)
                            {
                                if (tx * tx + ty * ty <= thickness * thickness) // Check if within circle radius for thickness
                                {
                                    WorldGen.PlaceTile(posX + tx, posY + ty, TileID.LivingWood, forced: true);
                                    WorldGen.PlaceTile(posX + (tx * 12), posY + (ty * 12), TileID.LeafBlock);
                                }
                            }
                        }
                    }
                }
            }
            public void GardenTunnel(int startX, int startY, int width, int maxDirChange, int endY)
            {
                int tunnelWidth = width;
                int maxDirectionChange = maxDirChange;

                int currentX = startX;
                int currentWidth = tunnelWidth;

                for (int y = startY; y <= endY; y++)
                {
                    int directionChange = Main.rand.Next(-maxDirectionChange, maxDirectionChange + 1);
                    currentX += directionChange;

                    int widthChange = Main.rand.Next(-1, 2);
                    currentWidth = Math.Max(3, currentWidth + widthChange);
                    for (int x = currentX - currentWidth / 2; x <= currentX + currentWidth / 2; x++)
                    {
                        WorldGen.KillTile(x, y, false, false, true);
                        WorldGen.KillWall(x, y, false);
                    }
                }
            }
            public void GenerateCellularAutomataCaves(int cX, int cY, int hR, int vR)
            {
                // Initialize the grid with random states within the oval
                bool[,] caveMap = new bool[hR * 2, vR * 2];
                for (int x = 0; x < hR * 2; x++)
                {
                    for (int y = 0; y < vR * 2; y++)
                    {
                        if (IsPointInsideEllipse(x + cX - hR, y + cY - vR, cX, cY, hR, vR))
                        {
                            caveMap[x, y] = Main.rand.Next(100) < 45; // Approximately 45% of the area will start as caves
                        }
                        else
                        {
                            caveMap[x, y] = false; // Ensure the edge remains solid
                        }
                    }
                }
                for (int iteration = 0; iteration < 5; iteration++)
                {
                    caveMap = PerformCellularAutomataStep(caveMap, hR * 2, vR * 2);
                }
                for (int x = 0; x < hR * 2; x++)
                {
                    for (int y = 0; y < vR * 2; y++)
                    {
                        if (caveMap[x, y])
                        {
                            int worldX = cX - hR + x;
                            int worldY = cY - vR + y;
                            WorldGen.KillTile(worldX, worldY, false, false, true); // This removes the tile, creating empty space
                            WorldGen.KillWall(worldX, worldY, false); // Optionally remove the wall for a complete cave effect
                        }
                    }
                }
            }
            private bool IsPointInsideEllipse(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius)
            {
                // The equation for an ellipse centered at (centerX, centerY) is:
                // ((x - centerX)^2 / horizontalRadius^2) + ((y - centerY)^2 / verticalRadius^2) <= 1
                // If the point (x, y) satisfies this inequality, it's inside the ellipse.

                float dx = (float)(x - centerX);
                float dy = (float)(y - centerY);
                return (dx * dx) / (horizontalRadius * horizontalRadius) + (dy * dy) / (verticalRadius * verticalRadius) <= 1;
            }
            private bool[,] PerformCellularAutomataStep(bool[,] map, int width, int height)
            {
                bool[,] newMap = new bool[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int solidNeighbors = CountSolidNeighbors(map, x, y, width, height);

                        // The rules of cellular automata
                        if (solidNeighbors > 4)
                            newMap[x, y] = true; // Tile becomes solid
                        else if (solidNeighbors < 4)
                            newMap[x, y] = false; // Tile becomes empty
                        else
                            newMap[x, y] = map[x, y]; // Remains the same
                    }
                }

                return newMap;
            }
            private int CountSolidNeighbors(bool[,] map, int x, int y, int width, int height)
            {
                int count = 0;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            // Skip the cell itself
                            continue;
                        }

                        int neighborX = x + i;
                        int neighborY = y + j;

                        // Check if neighbor is within bounds and solid
                        if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                        {
                            if (map[neighborX, neighborY])
                            {
                                count++;
                            }
                        }
                        else
                        {
                            // Treat out-of-bounds neighbors as solid
                            count++;
                        }
                    }
                }

                return count;
            }
        }
    }
}