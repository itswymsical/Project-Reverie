using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;
using Trelamium.Content.Tiles.DruidsGarden;
using Trelamium.Content.Tiles;
using Trelamium.Helpers;

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
        public class DruidaeaTreePass : GenPass
        {
            public DruidaeaTreePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Planting a divine seed (grab a drink)";

                #region Trunk Positioning and Generation
                int trunkX;
                int trunkDir = Main.rand.Next(2);
                int spawnX = Main.maxTilesX / 2;
                int spawnY = (int)Main.worldSurface - (Main.maxTilesY / 12);
                int distance = (Main.maxTilesX - spawnX) / 20;
                if (trunkDir == 0) {
                    trunkX = spawnX - distance;
                }
                else {
                    trunkX = spawnX + distance;
                }
                trunkX = Math.Clamp(trunkX, 0, Main.maxTilesX - 1); //safety

                int trunkTopY = (int)(spawnY - (Main.maxTilesY - spawnY) / 8);
                int trunkBottomY = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 8);
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
                #endregion

                #region Biome Base Positioning, Caves, etc.
                int centerX = spawnX;
                int centerY = trunkBottomY + (Main.maxTilesY - trunkBottomY) / 4;
                int horizontalRadius = (int)(Main.maxTilesX * 0.06375f);
                int verticalRadius = (int)(Main.maxTilesY * 0.215f);
                int topSectionEndY = centerY - verticalRadius / 3;
                int middleSectionEndY = centerY + verticalRadius / 3;

                for (int x = centerX - horizontalRadius; x <= centerX + horizontalRadius; x++)
                {
                    for (int y = centerY - verticalRadius; y <= centerY + verticalRadius; y++)
                    {
                        if (WorldGenHelpers.IsPointInsideEllipse(x, y, centerX, centerY, horizontalRadius, verticalRadius))
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
                        else if (WorldGenHelpers.IsPointNearOvalEdge(x, y, centerX, centerY, horizontalRadius, verticalRadius))
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
                WorldGenHelpers.GenerateCellularAutomataCaves(centerX, centerY, horizontalRadius, verticalRadius, 52, 12);
                WorldGenHelpers.GenerateCellularAutomataWalls(centerX, centerY, horizontalRadius, verticalRadius, 49, 9);
                #endregion

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, trunkX, trunkTopY, trunkWidth, trunkBottomY - trunkTopY + 1);
            }
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
        }

        public class DruidaeaExtrasPass : GenPass
        {
            public DruidaeaExtrasPass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Bringing life to the forest";
                #region Find Trunk Coords
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

                int trunkBottomY = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 7);

                int centerX = trunkX;
                int centerY = trunkBottomY + (Main.maxTilesY - trunkBottomY) / 4;
                int horizontalRadius = (int)(Main.maxTilesX * 0.058f);
                int verticalRadius = (int)(Main.maxTilesY * 0.305f);
                int middleSectionEndY = centerY + verticalRadius / 3;
                int topSectionEndY = centerY - verticalRadius / 3;
                #endregion

                int shrineCenterX = trunkX;
                int shrineCenterY = centerY;
                int shrineHorizontalRadius = (int)(Main.maxTilesX * 0.0095f);
                int shrineVerticalRadius = (int)(Main.maxTilesY * 0.0275f);
                int domeRadius = (int)(Main.maxTilesX * 0.0045f);

                for (int x = centerX - horizontalRadius; x <= centerX + horizontalRadius; x++)
                {
                    for (int y = centerY - verticalRadius; y <= centerY + verticalRadius; y++)
                    {
                        if (WorldGenHelpers.IsPointInsideEllipse(x, y, centerX, centerY, horizontalRadius, verticalRadius))
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
                        if (WorldGenHelpers.IsPointInsideEllipse(x, y, shrineCenterX, shrineCenterY, shrineHorizontalRadius, shrineVerticalRadius))
                        {
                            WorldGen.PlaceTile(x, y, TileID.LivingWood, forced: true);
                            WorldGen.KillWall(x, y);
                        }
                    }
                }

                WorldGenHelpers.GenerateCellularAutomataCaves(shrineCenterX, shrineCenterY, shrineHorizontalRadius, shrineVerticalRadius, 60, 12);
                /*
                for (int x = shrineCenterX - domeRadius; x <= shrineCenterX + domeRadius; x++)
                {
                    for (int y = shrineCenterY - domeRadius; y <= shrineCenterY; y++) // Only go up to the midpoint for a dome shape
                    {
                        if (WorldGenHelpers.IsPointInsideEllipse(x, y, shrineCenterX, shrineCenterY, domeRadius, domeRadius))
                        {
                            WorldGen.KillTile(x, y + 15);
                            WorldGen.PlaceWall(x, y + 15, WallID.LivingWoodUnsafe);
                        }
                    }
                } */
            }
        }

    }
}