using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;
using Trelamium.Content.Tiles.DruidsGarden;
using System.ComponentModel;
using Trelamium.Common;
using Microsoft.CodeAnalysis.Operations;
using System.Reflection.Metadata;
using Trelamium.Content.Tiles;

namespace Trelamium
{
    public class DruidaeaTreeSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int DruidaeaIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Altars"));
            if (DruidaeaIndex != 1)
            {
                tasks.Insert(DruidaeaIndex + 1, new DruidaeaTreePass("Druidaea Tree", 100f));
            }
            int DruidaeaExtrasIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Smooth World"));
            if (DruidaeaExtrasIndex != 1)
            {
                tasks.Insert(DruidaeaExtrasIndex + 1, new DruidaeaExtrasPass("Druidaea Extras", 100f));
            }
        }
        public class DruidaeaExtrasPass : GenPass
        {
            public DruidaeaExtrasPass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Bringing life to the forest";
                int trunkX;
                int trunkDir = Main.rand.Next(2);
                int spawnX = Main.maxTilesX / 2;
                int spawnY = (int)Main.worldSurface - (Main.maxTilesY / 16);
                int distance = (Main.maxTilesX - spawnX) / 20;
                if (trunkDir == 0)
                {
                    trunkX = spawnX - distance;
                }
                else
                {
                    trunkX = spawnX + distance;
                }
                trunkX = Math.Clamp(trunkX, 0, Main.maxTilesX - 1); //safety

                int trunkTopY = (int)(spawnY - (Main.maxTilesY - spawnY) / 8);
                int trunkBottomY = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 7);

                int centerX = trunkX;
                int centerY = trunkBottomY + (Main.maxTilesY - trunkBottomY) / 4;
                int horizontalRadius = (int)(Main.maxTilesX * 0.038f);
                int verticalRadius = (int)(Main.maxTilesY * 0.305f);
                int middleSectionEndY = centerY + verticalRadius / 3;
                int topSectionEndY = centerY - verticalRadius / 3;

                int shrineCenterX = trunkX;
                int shrineCenterY = centerY + (Main.maxTilesY - trunkBottomY) / 4;
                int shrineHorizontalRadius = (int)(Main.maxTilesX * 0.0105f);
                int shrineVerticalRadius = (int)(Main.maxTilesY * 0.0342f);

                for (int x = centerX - horizontalRadius; x <= centerX + horizontalRadius; x++)
                {
                    for (int y = centerY - verticalRadius; y <= centerY + verticalRadius; y++)
                    {
                        if (IsPointInsideEllipse(x, y, centerX, centerY, horizontalRadius, verticalRadius))
                        {
                            if (y <= middleSectionEndY)
                            {
                                WorldGen.SpreadGrass(x, y, TileID.Mud, TileID.MushroomGrass, true, default);                               
                            }
                            if (y <= topSectionEndY)
                            {
                                WorldGen.SpreadGrass(x, y, ModContent.TileType<LoamTile>(), ModContent.TileType<LoamTileGrass>(), true, default);
                            }
                        }
           
                    }
                }
                for (int x = shrineCenterX - shrineHorizontalRadius; x <= shrineCenterX + shrineHorizontalRadius; x++)
                {
                    for (int y = shrineCenterY - shrineVerticalRadius; y <= shrineCenterY + shrineVerticalRadius; y++)
                    {
                        if (IsPointInsideEllipse(x, y, shrineCenterX, shrineCenterY, shrineHorizontalRadius, shrineVerticalRadius))
                        {
                            WorldGen.PlaceTile(x, y, ModContent.TileType<SlateTile>(), forced: true);
                            WorldGen.KillWall(x, y);
                            WorldGen.KillTile(x / 3, y / 3);
                        }
                    }
                }
            }
            private static bool IsPointInsideEllipse(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius)
            {
                // The equation for an ellipse centered at (centerX, centerY) is:
                // ((x - centerX)^2 / horizontalRadius^2) + ((y - centerY)^2 / verticalRadius^2) <= 1
                // If the point (x, y) satisfies this inequality, it's inside the ellipse.

                float dx = (x - centerX);
                float dy = (y - centerY);
                return (dx * dx) / (horizontalRadius * horizontalRadius) + (dy * dy) / (verticalRadius * verticalRadius) <= 1;
            }
        }
        public class DruidaeaTreePass : GenPass
        {
            public DruidaeaTreePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Planting a divine seed";
                int trunkX;
                int trunkDir = Main.rand.Next(2);
                int spawnX = Main.maxTilesX / 2;
                int spawnY = (int)Main.worldSurface - (Main.maxTilesY / 16);
                int distance = (Main.maxTilesX - spawnX) / 20;
                if (trunkDir == 0) {
                    trunkX = spawnX - distance;
                }
                else {
                    trunkX = spawnX + distance;
                }
                trunkX = Math.Clamp(trunkX, 0, Main.maxTilesX - 1); //safety

                int trunkTopY = (int)(spawnY - (Main.maxTilesY - spawnY) / 8);
                int trunkBottomY = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 7);
                int trunkWidth = 18;

                const float curveFrequency = 0.0765f;
                const int curveAmplitude = 4;

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
                        WorldGen.KillWall(x, y);
                        WorldGen.PlaceTile(x, y, TileID.LivingWood, forced: true);
                    }
                }
                for (int y2 = trunkTopY; y2 <= trunkBottomY; y2++)
                {
                    int tunnelTrunkWidth = (trunkWidth / 2) + (y2 % 5 == 0 ? 2 : 0); // Example: add extra width every 5 blocks
                    int tunnelOffset = (int)(Math.Sin(y2 * curveFrequency) * curveAmplitude); // Calculate the curve offset
                    int leftBound = trunkX - tunnelTrunkWidth / 2 + tunnelOffset;
                    int rightBound = trunkX + tunnelTrunkWidth / 2 + tunnelOffset;
                    for (int x2 = leftBound; x2 <= rightBound; x2++)
                    {
                        WorldGen.KillTile(x2, y2);
                        WorldGen.PlaceWall(x2, y2, WallID.LivingWoodUnsafe);
                    }
                }
                
                GenerateLeaves(trunkX, trunkTopY, 4);

                int centerX = trunkX;
                int centerY = trunkBottomY + (Main.maxTilesY - trunkBottomY) / 4;
                int horizontalRadius = (int)(Main.maxTilesX * 0.02875f);
                int verticalRadius = (int)(Main.maxTilesY * 0.305f);
                int topSectionEndY = centerY - verticalRadius / 3;
                int middleSectionEndY = centerY + verticalRadius / 3;

                for (int x = centerX - horizontalRadius; x <= centerX + horizontalRadius; x++)
                {
                    for (int y = centerY - verticalRadius; y <= centerY + verticalRadius; y++)
                    {

                        if (IsPointInsideEllipse(x, y, centerX, centerY, horizontalRadius, verticalRadius))
                        {
                            WorldGen.KillWall(x, y);
                            
                            if (y <= topSectionEndY)
                            {
                                WorldGen.PlaceWall(x, y, WallID.LivingLeaf);
                                WorldGen.PlaceTile(x, y, ModContent.TileType<LoamTile>(), forced: true);
                                if (Main.rand.NextBool(62))
                                {
                                    WorldGen.TileRunner(x, y, Main.rand.Next(2, 4), Main.rand.Next(2, 4), ModContent.TileType<AlluviumOreTile>());
                                }
                                if (Main.rand.NextBool(65))
                                {
                                    WorldGen.TileRunner(x, y, Main.rand.Next(4, 9), Main.rand.Next(5, 7), ModContent.TileType<CobblestoneTile>());
                                }

                            }
                            else if (y <= middleSectionEndY)
                            {
                                WorldGen.PlaceWall(x, y, WallID.MudUnsafe);
                                WorldGen.PlaceTile(x, y, TileID.Mud, forced: true);
                            }
                            else
                            {
                                WorldGen.PlaceWall(x, y, WallID.Cave2Unsafe);
                                WorldGen.PlaceTile(x, y, ModContent.TileType<SlateTile>(), forced: true);
                                if (Main.rand.NextBool(40))
                                {
                                    WorldGen.TileRunner(x, y, Main.rand.Next(3, 7), Main.rand.Next(3, 7), ModContent.TileType<AlluviumOreTile>());
                                }
                            }
                        }
                        else if (IsPointNearOvalEdge(x, y, centerX, centerY, horizontalRadius, verticalRadius))
                        {
                            if (Main.rand.NextFloat() < 0.8f)
                            {
                                int branchLength = Main.rand.Next(32, 61); // Randomize the branch length
                                for (int i = 0; i < branchLength; i++)
                                {
                                    int branchX = x + Main.rand.Next(-1, 2); // Randomize the branch direction
                                    int branchY = y + Main.rand.Next(-1, 2);
                                    if (!WorldGen.TileEmpty(branchX, branchY))
                                    {
                                        if (y <= topSectionEndY)
                                        {
                                            WorldGen.PlaceTile(branchX, branchY, ModContent.TileType<LoamTile>(), forced: true);
                                        }
                                        else if (y <= middleSectionEndY)
                                        {
                                            WorldGen.PlaceTile(branchX, branchY, TileID.Mud, forced: true);                                      
                                        }
                                        else
                                        {
                                            WorldGen.PlaceTile(branchX, branchY, ModContent.TileType<SlateTile>(), forced: true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                GenerateCellularAutomataCaves(centerX, centerY, horizontalRadius, verticalRadius, 52, 12);

                GenerateCellularAutomataWalls(centerX, centerY, horizontalRadius, verticalRadius, 49, 9);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, trunkX, trunkTopY, trunkWidth, trunkBottomY - trunkTopY + 1);
            }
            #region Cellular Automata
            public static void GenerateCellularAutomataCaves(int cX, int cY, int hR, int vR, int density, int iterations)
            {
                bool[,] caveMap = new bool[hR * 2, vR * 2];
                for (int x = 0; x < hR * 2; x++)
                {
                    for (int y = 0; y < vR * 2; y++)
                    {
                        if (IsPointInsideEllipse(x + cX - hR, y + cY - vR, cX, cY, hR, vR))
                        {
                            caveMap[x, y] = Main.rand.Next(100) < density;
                        }
                        else
                        {
                            caveMap[x, y] = false;
                        }
                    }
                }
                for (int iteration = 0; iteration < iterations; iteration++)
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
                            WorldGen.KillTile(worldX, worldY); // This removes the tile, creating empty space  
                        }
                    }
                }
                
            }
            public static void GenerateCellularAutomataWalls(int cX, int cY, int hR, int vR, int density, int iterations)
            {
                // Initialize the grid with random states within the oval
                bool[,] caveMap = new bool[hR * 2, vR * 2];
                for (int x = 0; x < hR * 2; x++)
                {
                    for (int y = 0; y < vR * 2; y++)
                    {
                        if (IsPointInsideEllipse(x + cX - hR, y + cY - vR, cX, cY, hR, vR))
                        {
                            caveMap[x, y] = Main.rand.Next(100) < density;
                        }
                        else
                        {
                            caveMap[x, y] = false;
                        }
                    }
                }
                for (int iteration = 0; iteration < iterations; iteration++)
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
                            WorldGen.KillWall(worldX, worldY, false);         
                        }
                    }
                }
            }
            private static bool[,] PerformCellularAutomataStep(bool[,] map, int width, int height)
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
            private static int CountSolidNeighbors(bool[,] map, int x, int y, int width, int height)
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
            #endregion

            #region Biome Base
            public static void GenerateLeaves(int trunkX, int trunkY, int thickness)
            {
                float[] angles = new float[] { MathHelper.ToRadians(-120), MathHelper.ToRadians(-90), MathHelper.ToRadians(-45), MathHelper.ToRadians(45), MathHelper.ToRadians(90), MathHelper.ToRadians(120) };

                int numBranches = angles.Length;

                for (int i = 0; i < numBranches; i++)
                {
                    float angle = angles[i];
                    int branchLength = 32;

                    // Apply the curves as we did previously, with a sine wave function
                    for (int j = 0; j < branchLength; j++)
                    {
                        // Apply a gentle curvature using a sine wave
                        float curve = (float)Math.Sin(j * 0.09f) * 3f;  // Adjust the frequency and amplitude as needed

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
                                    WorldGen.TileRunner(posX + tx, posY + ty, 30, 30, TileID.LeafBlock, true, 1, 1);
                                }
                            }
                        }
                    }
                }
            }
            private static bool IsPointInsideEllipse(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius)
            {
                // The equation for an ellipse centered at (centerX, centerY) is:
                // ((x - centerX)^2 / horizontalRadius^2) + ((y - centerY)^2 / verticalRadius^2) <= 1
                // If the point (x, y) satisfies this inequality, it's inside the ellipse.

                float dx = (x - centerX);
                float dy = (y - centerY);
                return (dx * dx) / (horizontalRadius * horizontalRadius) + (dy * dy) / (verticalRadius * verticalRadius) <= 1;
            }
            private static bool IsPointNearOvalEdge(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius, float threshold = 0.1f)
            {
                // Calculate the normalized distance from the point to the ellipse's edge
                float dx = (float)(x - centerX) / horizontalRadius;
                float dy = (float)(y - centerY) / verticalRadius;
                float distance = dx * dx + dy * dy;

                // Check if the point is near the edge of the ellipse
                return distance >= (1.0f - threshold) && distance <= (1.0f + threshold);
            }

            #endregion
           
        }

    }
}