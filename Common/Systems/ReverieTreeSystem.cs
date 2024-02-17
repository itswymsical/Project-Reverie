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

namespace ReverieMod
{
    public class ReverieSystem : ModSystem
    {
        public class ReveriePass : GenPass
        {
            public ReveriePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Manifesting Reverie";

                #region Trunk Positioning and Generation
                int trunkX;
                int trunkDir = Main.rand.Next(2);
                int spawnX = Main.maxTilesX / 2;
                int spawnY = (int)Main.worldSurface - (Main.maxTilesY / 12);
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
                int trunkBottomY = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 8);
                int trunkWidth = 10;
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
                int centerX = trunkX;
                int centerY = trunkBottomY + (Main.maxTilesY - trunkBottomY) / 4;
                int horizontalRadius = (int)(Main.maxTilesX * 0.035f);
                int verticalRadius = (int)(Main.maxTilesY * 0.175f);

                for (int x = centerX - horizontalRadius; x <= centerX + horizontalRadius; x++)
                {
                    for (int y = centerY - verticalRadius; y <= centerY + verticalRadius; y++)
                    {
                        if (WorldGenHelpers.IsPointInsideEllipse(x, y, centerX, centerY, horizontalRadius, verticalRadius))
                        {
                            WorldGen.KillWall(x, y);
                            WorldGen.PlaceWall(x, y, WallID.LivingLeaf);
                            WorldGen.PlaceTile(x, y, 0, forced: true);
                            if (Main.rand.NextBool(64))
                            {
                                WorldGen.TileRunner(x, y, Main.rand.Next(2, 4), Main.rand.Next(2, 4), ModContent.TileType<AlluviumOreTile>());
                            }
                            if (Main.rand.NextBool(67))
                            {
                                WorldGen.TileRunner(x, y, Main.rand.Next(4, 7), Main.rand.Next(4, 7), ModContent.TileType<CobblestoneTile>());
                            }
                        }
                        else if (WorldGenHelpers.IsPointNearOvalEdge(x, y, centerX, centerY, horizontalRadius, verticalRadius))
                        {
                            if (Main.rand.NextFloat() < 0.8f)
                            {
                                int branchLength = Main.rand.Next(21, 30); // Randomize the branch length
                                for (int i = 0; i < branchLength; i++)
                                {
                                    int branchX = x + Main.rand.Next(-1, 2); // Randomize the branch direction
                                    int branchY = y + Main.rand.Next(-1, 2);
                                    if (!WorldGen.TileEmpty(branchX, branchY))
                                    {
                                        WorldGen.PlaceTile(branchX, branchY, 0, forced: true);
                                    }
                                }
                            }
                        }
                    }
                }
                int cellX = horizontalRadius - (horizontalRadius / 12);
                int cellY = verticalRadius - (verticalRadius / 12);
                WorldGenHelpers.GenerateCellularAutomata(centerX, centerY, cellX, cellY, 48, 8, true, 0, false);
                WorldGenHelpers.GenerateCellularAutomataWalls(centerX, centerY, cellX, cellY, 42, 9);
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
    
        public class ReverieExtrasPass : GenPass
        {
            public ReverieExtrasPass(string name, float loadWeight) : base(name, loadWeight)
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

                int controlPointOffsetX = Main.rand.Next(-40, 40); // Random horizontal offset
                int controlPointOffsetY = Main.rand.Next(-20, 0);  // Random vertical offset, upwards

                Vector2 start = new Vector2(trunkX, trunkBottomY);
                Vector2 control = new Vector2(trunkX + controlPointOffsetX, trunkBottomY + controlPointOffsetY);
                Vector2 end = new Vector2(trunkX, trunkBottomY - 50);

                GenerateTreeRoot(start, control, end, TileID.LivingWood);
                int centerX = trunkX;
                int centerY = trunkBottomY + (Main.maxTilesY - trunkBottomY) / 4;
                int horizontalRadius = (int)(Main.maxTilesX * 0.06025f);
                int verticalRadius = (int)(Main.maxTilesY * 0.215f);
                int middleSectionEndY = centerY + verticalRadius / 3;
                int topSectionEndY = centerY - verticalRadius / 3;
                #endregion

                int shrineCenterX = trunkX;
                int shrineCenterY = topSectionEndY;
                int shrineHorizontalRadius = (int)(Main.maxTilesX * 0.0095f);
                int shrineVerticalRadius = (int)(Main.maxTilesY * 0.0275f);
                int domeRadius = (int)(Main.maxTilesX * 0.0095f);
                
                

                
                for (int x = centerX - horizontalRadius; x <= centerX + horizontalRadius; x++)
                {
                    for (int y = centerY - verticalRadius; y <= centerY + verticalRadius; y++)
                    {
                        progress.Set(x / horizontalRadius);
                        if (WorldGenHelpers.IsPointInsideEllipse(x, y, centerX, centerY, horizontalRadius, verticalRadius))
                        {
                            //WorldGen.SpreadGrass(x, y, 0, ModContent.TileType<WoodlandGrassTile>(), true, default);

                            Tile tile = Framing.GetTileSafely(x, y);
                            for (int grassX = x - 1; grassX <= x + 1; grassX++)
                            {
                                for (int grassY = y - 1; grassY <= y + 1; grassY++)
                                {
                                    Tile tile2 = Framing.GetTileSafely(grassX, grassY);
                                    if (!tile2.HasTile)
                                    {
                                        if (tile.TileType == TileID.Dirt || TileID.Sets.Grass[tile.TileType])
                                            tile.TileType = (ushort)ModContent.TileType<WoodlandGrassTile>();

                                        if (tile.HasTile && tile2.WallType == 0)
                                            tile.WallType = 0;
                                    }
                                }
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

                WorldGenHelpers.GenerateCellularAutomata(shrineCenterX, shrineCenterY, shrineHorizontalRadius, shrineVerticalRadius, 30, 12, true, 0, false);

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
                }
            }
            Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
            {
                float u = 1 - t;
                float tt = t * t;
                float uu = u * u;

                Vector2 p = uu * p0; // First term
                p += 2 * u * t * p1; // Second term
                p += tt * p2; // Third term

                return p;
            }

            // Generate tree root
            void GenerateTreeRoot(Vector2 p0, Vector2 p1, Vector2 p2, ushort tileType)
            {
                for (float t = 0; t <= 1; t += 0.01f) // Increment t to draw the curve
                {
                    Vector2 point = CalculateBezierPoint(t, p0, p1, p2);
                    WorldGen.PlaceTile((int)point.X, (int)point.Y, tileType, mute: true, forced: true);
                }
            }

        }
        public class ForestTemplePass : GenPass
        {
            public ForestTemplePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Ligneous Temple";
                int spawnX = Main.maxTilesX / 2;

                int trunkBottomY = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 8);

                int centerX = spawnX;
                int centerY = trunkBottomY + (Main.maxTilesY - trunkBottomY) / 4;
                int horizontalRadius = (int)(Main.maxTilesX * 0.06025f);
                int verticalRadius = (int)(Main.maxTilesY * 0.215f);

                for (int x = centerX - horizontalRadius; x <= centerX + horizontalRadius; x++)
                {
                    for (int y = centerY - verticalRadius; y <= centerY + verticalRadius; y++)
                    {
                        if (WorldGenHelpers.IsPointInsideEllipse(x, y, centerX, centerY, horizontalRadius, verticalRadius))
                        {

                            if (Main.rand.NextBool(210))
                            {
                                WorldGen.digTunnel(x, y, Main.rand.Next(1, 5), Main.rand.Next(1, 3), Main.rand.Next(1, 3), Main.rand.Next(3, 4));
                            }
                        }
                    }
                }
            }
        }
    }
}