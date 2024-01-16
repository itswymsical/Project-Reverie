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

namespace Trelamium
{
    public class DruidaeaTreeSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int DruidaeaIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Guide"));
            if (DruidaeaIndex != 1)
            {
                tasks.Insert(DruidaeaIndex + 1, new DruidaeaTreePass("Druidaea Tree", 100f));
            }
        }
        public class DruidaeaTreePass : GenPass
        {
            private const float MAX_ANGLE_OFFSET = 0.2f;
            private const int MIN_LEAF_DENSITY = 5;
            private const int MAX_LEAF_DENSITY = 15;
            private const float LEAF_RADIUS = 3.0f;
            public DruidaeaTreePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Planting a druidaea seed";
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

                int trunkTopY = (int)(Main.spawnTileY - (Main.maxTilesY - Main.spawnTileY) / 8);
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
                        WorldGen.PlaceTile(x, y, TileID.LivingWood, forced: true);
                        WorldGen.KillTile(x / 2, y / 2);
                    }
                }

                //CreateTrunkTunnel(trunkX, trunkTopY, trunkBottomY - 60);
                GenerateLeaves(new Vector2(trunkX, trunkTopY), Main.rand.Next(-5, 6), Main.rand.Next(0, 256));
                GenerateBranches(trunkX, trunkTopY, 10, 1);
                int centerX = trunkX;
                int centerY = trunkBottomY;
                int horizontalRadius = (int)(Main.maxTilesX * 0.035f); //the world has way more 'X' tiles so lower the %
                int verticalRadius = (int)(Main.maxTilesY * 0.125f); //12.5%
                int topSectionEndY = centerY - verticalRadius / 3; // The end Y-coordinate of the top section
                int middleSectionEndY = centerY + verticalRadius / 3; // The end Y-coordinate of the middle section

                for (int x = centerX - horizontalRadius; x <= centerX + horizontalRadius; x++)
                {
                    for (int y = centerY - verticalRadius; y <= centerY + verticalRadius; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile)
                        {
                            WorldGen.PlaceTile(x, y, TileID.Vines, forced: true);
                        }

                        if (IsPointInsideEllipse(x, y, centerX, centerY, horizontalRadius, verticalRadius))
                        {

                            if (y <= topSectionEndY)
                            {
                                WorldGen.PlaceWall(x, y, WallID.FlowerUnsafe);
                                WorldGen.PlaceTile(x, y, ModContent.TileType<LoamTile>(), forced: true);
                            }
                            else if (y <= middleSectionEndY)
                            {
                                WorldGen.PlaceWall(x, y, WallID.Cave2Unsafe);
                                WorldGen.PlaceTile(x, y, ModContent.TileType<SlateTile>(), forced: true);
                            }
                            else
                            {
                                WorldGen.PlaceWall(x, y, WallID.Sandstone);
                                WorldGen.PlaceTile(x, y, TileID.Sandstone, forced: true);
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
                                        WorldGen.PlaceTile(branchX, branchY, ModContent.TileType<LoamTile>(), forced: true);
                                        WorldGen.PlaceWall(centerX, centerY, WallID.None);
                                    }
                                }
                            }
                        }
                    }
                }
                //GenerateSineWaveCaves(centerX, centerY, horizontalRadius);
                GenerateCellularAutomataCaves(centerX, centerY, horizontalRadius, verticalRadius, 48);

                GenerateMushroomShape(centerX, centerY, horizontalRadius, verticalRadius);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, trunkX, trunkTopY, trunkWidth, trunkBottomY - trunkTopY + 1);
            }
            #region Tree Branches, Leaves, and Tunnels
            /*public static void GenerateLeaves(int trunkX, int trunkY, int numBranches, int thickness)
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
                                    WorldGen.TileRunner(posX + tx, posY + ty, 30, 30, TileID.LeafBlock, true, 1, 1, ignoreTileType: TileID.LivingWood);
                                }
                            }
                        }
                    }
                }
            }*/
            public static void GenerateLeaves(Vector2 branchEnd, float scale, int seed)
            {
                // Determine the number of leaves to be generated around this branch end.
                float noiseValue = (float)PerlinNoise.Generate(branchEnd.X * scale, branchEnd.Y * scale, seed);
                int leafDensity = MIN_LEAF_DENSITY + (int)((MAX_LEAF_DENSITY - MIN_LEAF_DENSITY) * noiseValue);

                for (int i = 0; i < leafDensity; i++)
                {
                    // Calculate a random position for each leaf around the branch end
                    Vector2 leafPosition = GetRandomPositionAround(branchEnd, LEAF_RADIUS);
                    PlaceLeafTile(leafPosition);
                }
            }

            private static Vector2 GetRandomPositionAround(Vector2 position, float radius)
            {
                // Randomize angle and distance from the branch end to place the leaf
                double angle = Main.rand.NextDouble() * Math.PI * 2;
                double distance = Main.rand.NextDouble() * radius;

                // Calculate the leaf's position
                Vector2 leafPosition = position + new Vector2((float)(distance * Math.Cos(angle)), (float)(distance * Math.Sin(angle)));
                return leafPosition;
            }
            private static void PlaceLeafTile(Vector2 position)
            {
                Tile tile = Main.tile[(int)position.X, (int)position.Y];
                if (tile == null || tile.HasTile)
                {
                    return; // There's already a tile here or it's null, so we don't place a leaf
                }

                tile.TileType = TileID.LeafBlock; // Set the tile type to your leaf type
                tile.HasTile = true; // Activate the tile to make it visible
                                   // You might need to set other properties of the tile here depending on your needs
            }
            public static void GenerateBranches(int trunkX, int trunkY, int numBranches, int thickness)
            {
                float[] angles = new float[] { MathHelper.ToRadians(-120), MathHelper.ToRadians(-90), MathHelper.ToRadians(-45), MathHelper.ToRadians(45), MathHelper.ToRadians(90), MathHelper.ToRadians(120) };

                numBranches = angles.Length;
                for (int i = 0; i < numBranches; i++)
                {
                    float angle = angles[i];
                    int branchLength = 56; // Consider using Perlin noise to vary this too.

                    // Generate each segment of the branch...
                    for (int j = 0; j < branchLength; j++)
                    {
                        // Use Perlin noise to adjust the angle and length of each branch segment
                        double noiseValue = PerlinNoise.Generate(trunkX + j, trunkY, 0);
                        float noiseAngleOffset = (float)(MAX_ANGLE_OFFSET * (noiseValue - 0.5));
                        float finalAngle = angle + noiseAngleOffset;

                        // Existing logic for placing branch tiles...
                        int posX = trunkX + (int)(j * Math.Cos(finalAngle));
                        int posY = trunkY - (int)(j * Math.Sin(finalAngle));

                        // Existing logic for branch thickness...
                        for (int tx = -thickness; tx <= thickness; tx++)
                        {
                            for (int ty = -thickness; ty <= thickness; ty++)
                            {
                                if (tx * tx + ty * ty <= thickness * thickness) // Check if within circle radius for thickness
                                {
                                    WorldGen.PlaceTile(posX + tx, posY + ty, TileID.LivingWood, forced: true);
                                }
                            }
                        }
                    }
                }
            }
           
            public static void CreateTrunkTunnel(int trunkX, int trunkTopY, int depthToCaves)
            {
                int tunnelStartY = trunkTopY + 50; // Starting point of the tunnel from the tree top
                int tunnelEndY = tunnelStartY + depthToCaves; // The depth where the tunnel connects to the cave system
                int currentX = trunkX; // Starting X position of the tunnel

                for (int y = tunnelStartY; y <= tunnelEndY; y++)
                {
                    // Varying the width and direction for a non-uniform look
                    int tunnelWidth = Main.rand.Next(4, 7); // Random tunnel width for natural variation
                    currentX += Main.rand.Next(-1, 2); // Slight random shift in the X direction
                                                       // Clamp currentX to ensure it stays within world bounds
                    currentX = Math.Clamp(currentX, 0, Main.maxTilesX - 1);
                    // Carve out the tunnel
                    for (int x = currentX - tunnelWidth / 2; x <= currentX + tunnelWidth / 2; x++)
                    {
                        // Ensure x is within world bounds
                        if (x >= 0 && x < Main.maxTilesX)
                        {
                            WorldGen.KillTile(x, y, false, false, true); // Remove tiles to create the tunnel
                            WorldGen.KillWall(x, y, false); // Optionally, remove the wall for a complete tunnel
                            if (WorldGen.TileEmpty(x, y))
                            {
                                WorldGen.PlaceWall(x, y, WallID.LivingWoodUnsafe);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Cellular Automata
            public static void GenerateCellularAutomataCaves(int cX, int cY, int hR, int vR, int startPercentage)
            {
                // Initialize the grid with random states within the oval
                bool[,] caveMap = new bool[hR * 2, vR * 2];
                for (int x = 0; x < hR * 2; x++)
                {
                    for (int y = 0; y < vR * 2; y++)
                    {
                        if (IsPointInsideEllipse(x + cX - hR, y + cY - vR, cX, cY, hR, vR))
                        {
                            caveMap[x, y] = Main.rand.Next(100) < startPercentage; // Approximately 45% of the area will start as caves
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
                            WorldGen.KillTile(worldX, worldY); // This removes the tile, creating empty space          
                        }
                    }
                }
            }
            private static bool IsPointInsideEllipse(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius)
            {
                // The equation for an ellipse centered at (centerX, centerY) is:
                // ((x - centerX)^2 / horizontalRadius^2) + ((y - centerY)^2 / verticalRadius^2) <= 1
                // If the point (x, y) satisfies this inequality, it's inside the ellipse.

                float dx = (float)(x - centerX);
                float dy = (float)(y - centerY);
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
            public void RemoveFloatingTiles(int startX, int startY, int width, int height)
            {
                for (int x = startX; x < startX + width; x++)
                {
                    for (int y = startY; y < startY + height; y++)
                    {
                        if (WorldGen.TileEmpty(x, y))
                        {
                            continue; // Skip empty tiles
                        }

                        // Check adjacent tiles
                        bool hasAdjacentTile =
                            (!WorldGen.TileEmpty(x - 1, y)) || // Left
                            (!WorldGen.TileEmpty(x + 1, y)) || // Right
                            (!WorldGen.TileEmpty(x, y - 1)) || // Top
                            (!WorldGen.TileEmpty(x, y + 1));   // Bottom

                        // Remove tile if it does not have adjacent tiles
                        if (!hasAdjacentTile)
                        {
                            WorldGen.KillTile(x, y, false, false, false);
                        }
                    }
                }
            }
            #endregion
            public static void GenerateMushroomShape(int centerX, int centerY, int horizontalRadius, int verticalRadius)
            {
                int mushroomX = centerX - horizontalRadius / 2;
                int mushroomY = centerY - verticalRadius / 3;

                int capRadiusX = horizontalRadius / 3;
                int capRadiusY = verticalRadius / 3;

                for (int x = mushroomX - capRadiusX; x <= mushroomX + capRadiusX; x++)
                {
                    for (int y = mushroomY - capRadiusY; y <= mushroomY + capRadiusY; y++)
                    {
                        if (IsPointInsideEllipseMush(x, y, mushroomX, mushroomY, capRadiusX, capRadiusY))
                        {
                            WorldGen.PlaceTile(x, y, TileID.MushroomBlock, forced: true); // Use the appropriate TileID for mushroom blocks
                        }
                    }
                }
                GenerateCellularAutomataCaves(mushroomX, mushroomY, capRadiusX, capRadiusY, 50);
            }
            private static bool IsPointInsideEllipseMush(int x, int y, int centerX, int centerY, int radiusX, int radiusY)
            {
                return ((float)(x - centerX) * (x - centerX)) / (radiusX * radiusX) +
                       ((float)(y - centerY) * (y - centerY)) / (radiusY * radiusY) <= 1;
            }
        }

    }
}