using Microsoft.Xna.Framework;
using ReverieMod.Content.Tiles.WoodlandCanopy;
using ReverieMod.Utilities;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace ReverieMod.Common.Systems
{
    public class CanopyWorldGen : ModSystem
    {
        public static int TRUNK_X;
        public static int treeWood = TileID.LivingWood;
        public static int treeWall = WallID.LivingWoodUnsafe;
        public static int canopyWall = WallID.DirtUnsafe3;
        public static int treeLeaves = TileID.LeafBlock;
        public static int canopyGrass = ModContent.TileType<WoodlandGrassTile>();
        public static int canopyVines = ModContent.TileType<CanopyVine>();

        public static void Gen_NoiseMap(int cX, int cY, int hR, int vR, int density, int iterations, bool killTile, int type, bool forced)
        {
            bool[,] caveMap = new bool[hR * 2, vR * 2];
            for (int x = 0; x < hR * 2; x++)
            {
                for (int y = 0; y < vR * 2; y++)
                {
                    caveMap[x, y] = Main.rand.Next(100) < density;

                }
            }
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                caveMap = PerformStep(caveMap, hR * 2, vR * 2);
            }
            for (int x = 0; x < hR * 2; x++)
            {
                for (int y = 0; y < vR * 2; y++)
                {
                    if (caveMap[x, y])
                    {
                        int worldX = cX - hR + x;
                        int worldY = cY - vR + y;
                        if (killTile)
                        {
                            WorldGen.KillTile(worldX, worldY);
                        }
                        else
                        {
                            WorldGen.PlaceTile(worldX, worldX, type, forced: forced);
                        }
                    }
                }
            }

        }
        public static void Gen_NoiseMap_Walls(int cX, int cY, int hR, int vR, int density, int iterations)
        {
            bool[,] caveMap = new bool[hR * 2, vR * 2];
            for (int x = 0; x < hR * 2; x++)
            {
                for (int y = 0; y < vR * 2; y++)
                {

                    caveMap[x, y] = Main.rand.Next(100) < density;
                }
            }
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                caveMap = PerformStep(caveMap, hR * 2, vR * 2);
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
        private static bool[,] PerformStep(bool[,] map, int width, int height)
        {
            bool[,] newMap = new bool[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int solidNeighbors = CountSolidNeighbors(map, x, y, width, height);

                    if (solidNeighbors > 4)
                        newMap[x, y] = true;
                    else if (solidNeighbors < 4)
                        newMap[x, y] = false;
                    else
                        newMap[x, y] = map[x, y];
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
                        continue;
                    }

                    int neighborX = x + i;
                    int neighborY = y + j;

                    if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                    {
                        if (map[neighborX, neighborY])
                        {
                            count++;
                        }
                    }
                    else
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        //TODO: Create a fractal Tree or use L-systems to procedurally generate a detailed tree.
        public class CanopyPass : GenPass
        {
            public CanopyPass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Soiling the underground";
                #region VARIABLES
                int TRUNK_DIR = Main.rand.Next(2);

                int SPAWN_X = Main.maxTilesX / 2;
                int SPAWN_Y = (int)Main.worldSurface;
                int SPAWN_DISTANCE = (Main.maxTilesX - SPAWN_X) / 20;
                if (TRUNK_DIR == 0)
                {
                    TRUNK_X = SPAWN_X - SPAWN_DISTANCE;
                }
                else
                {
                    TRUNK_X = SPAWN_X + SPAWN_DISTANCE;
                }
                int TRUNK_BOTTOM = (int)(SPAWN_Y + (SPAWN_Y / 2));

                int CANOPY_X = SPAWN_X;
                int CANOPY_Y = TRUNK_BOTTOM;

                //Horizontal and Vertical Radius
                int CANOPY_H = (int)(Main.maxTilesX * 0.035f);
                int CANOPY_V = (int)(Main.maxTilesY * 0.140f);

                TRUNK_X = Math.Clamp(TRUNK_X, 0, Main.maxTilesX - 1); //safety
                #endregion

                int[] protectedTiles = new int[] { TileID.Sand, TileID.Sandstone, TileID.HardenedSand};

                //calculate the area and radius
                for (int x = CANOPY_X - CANOPY_H; x <= CANOPY_X + CANOPY_H; x++)
                {
                    for (int y = CANOPY_Y - CANOPY_V; y <= CANOPY_Y + CANOPY_V; y++)
                    {
                        WorldGen.KillWall(x, y);
                        WorldGen.PlaceWall(x, y, canopyWall);
                        WorldGen.TileRunner(x, y, 20, 5, treeWood, true, 0, 0, false, true);
                        //WorldGen.PlaceTile(x, y, treeWood, forced: true);
                        if (Main.rand.NextFloat() < 0.8f)
                        {
                            int border = Main.rand.Next(21, 30);
                            for (int i = 0; i < border; i++)
                            {
                                int borderX = x + Main.rand.Next(-2, 4);
                                int borderY = y + Main.rand.Next(-2, 4);
                               
                                WorldGen.PlaceTile(borderX + Main.rand.Next(-2, 4), borderY + Main.rand.Next(-2, 4), treeWood, forced: true); //this is a blend effect emulating noise around the biome

                            }
                        }
                        //this is the blue progress bar that tracks the actual speed of the generation
                        progress.Set((float)((x - (CANOPY_X - CANOPY_H)) * (2 * CANOPY_V) + (y - (CANOPY_Y - CANOPY_V))) / ((2 * CANOPY_H) * (2 * CANOPY_V)));
                    }
                }
                FastNoiseLite noise = new FastNoiseLite();
                noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
                noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
                noise.SetFractalGain(0.325f);
                noise.SetFractalOctaves(5);
                noise.SetFrequency(0.022f);

                int posx = CANOPY_H * 2;
                int posy = CANOPY_V * 2;
                float threshold = 0.47f; // Define your noise threshold
                                        // Gather noise data
                float[,] noiseData = new float[posx, posy];
                for (int x = 0; x < posx; x++)
                {
                    for (int y = 0; y < posy; y++)
                    {
                        int worldX = x + (CANOPY_X - CANOPY_H);
                        int worldY = y + (CANOPY_Y - CANOPY_V);

                        noiseData[x, y] = noise.GetNoise(worldX, worldY);
                    }
                }

                // Apply noise data to world coordinates within the canopy
                for (int x = 0; x < posx; x++)
                {
                    if (x >= 0 && x <= 1)
                    {
                        progress.Message = "Overgrowing tree roots";
                    }

                    for (int y = 0; y < posy; y++)
                    {
                        int worldX = x + (CANOPY_X - CANOPY_H);
                        int worldY = y + (CANOPY_Y - CANOPY_V);

                        if (noiseData[x, y] < threshold) // '>' = dark values, '<' = light values
                        {
                            if (Main.tile[worldX, worldY].HasTile)
                            {
                                WorldGen.KillTile(worldX, worldY);
                            }
                        }

                        // Update progress
                        float progressPercentage = (float)((x * posy + y) + (posx * posy)) / (2 * posx * posy);
                        progress.Set(progressPercentage);
                    }
                }
                Gen_NoiseMap_Walls(CANOPY_X, CANOPY_Y, CANOPY_H - (CANOPY_H / 128), CANOPY_V - (CANOPY_V / 128), 46, 8);
            }
            //TODO: Rewrite the leaves with the new fractal tree
        }
        public class ReverieTreePass : GenPass
        {
            public ReverieTreePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Weathering forest caverns";
                #region VARIABLES
                int TRUNK_DIR = Main.rand.Next(2);

                int SPAWN_X = Main.maxTilesX / 2;
                int SPAWN_Y = (int)Main.worldSurface;
                int SPAWN_DISTANCE = (Main.maxTilesX - SPAWN_X) / 20;
                if (TRUNK_DIR == 0)
                {
                    TRUNK_X = SPAWN_X - SPAWN_DISTANCE;
                }
                else
                {
                    TRUNK_X = SPAWN_X + SPAWN_DISTANCE;
                }
                int TRUNK_WIDTH = 12;
                int TRUNK_TOP = (int)(SPAWN_Y - (SPAWN_Y / 2));
                int TRUNK_BOTTOM = (int)(SPAWN_Y + (SPAWN_Y / 4));

                int CANOPY_X = SPAWN_X;
                int CANOPY_Y = TRUNK_BOTTOM;

                //Horizontal and Vertical Radius
                int CANOPY_H = (int)(Main.maxTilesX * 0.035f);
                int CANOPY_V = (int)(Main.maxTilesY * 0.140f);

                TRUNK_X = Math.Clamp(TRUNK_X, 0, Main.maxTilesX - 1); //safety
                #endregion

                //setting the sine wave variables
                const float TRUNK_CURVE_FREQUENCY = 0.0765f;
                const int TRUNK_CURVE_AMPLITUDE = 4;
                //GenerateCaverns(CANOPY_X, CANOPY_Y);
                //Placing trunk wood
                for (int y = TRUNK_TOP; y <= TRUNK_BOTTOM; y++)
                {
                    int currentTRUNK_WIDTH = TRUNK_WIDTH + (y % 5 == 0 ? 2 : 0);
                    int curveOffset = (int)(Math.Sin(y * TRUNK_CURVE_FREQUENCY) * TRUNK_CURVE_AMPLITUDE);

                    int leftBound = TRUNK_X - currentTRUNK_WIDTH / 2 + curveOffset;
                    int rightBound = TRUNK_X + currentTRUNK_WIDTH / 2 + curveOffset;

                    for (int x = leftBound; x <= rightBound; x++)
                    {
                        WorldGen.KillWall(x, y);
                        WorldGen.PlaceTile(x, y, treeWood, mute: true, forced: true);

                    }
                }
                //Placing trunk wall
                for (int y2 = TRUNK_TOP; y2 <= TRUNK_BOTTOM; y2++)
                {
                    int tunnelTRUNK_WIDTH = (TRUNK_WIDTH / 2) + (y2 % 5 == 0 ? 2 : 0);
                    int tunnelOffset = (int)(Math.Sin(y2 * TRUNK_CURVE_FREQUENCY) * TRUNK_CURVE_AMPLITUDE);
                    int leftBound = TRUNK_X - tunnelTRUNK_WIDTH / 2 + tunnelOffset;
                    int rightBound = TRUNK_X + tunnelTRUNK_WIDTH / 2 + tunnelOffset;
                    for (int x2 = leftBound; x2 <= rightBound; x2++)
                    {
                        WorldGen.KillTile(x2, y2);
                        WorldGen.PlaceWall(x2, y2, treeWall);
                    }
                }
                //the tree top of reverie
                GenerateLeaves(TRUNK_X, TRUNK_TOP, 4);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, TRUNK_X, TRUNK_TOP, TRUNK_WIDTH, TRUNK_BOTTOM - TRUNK_TOP + 1); //tile rect sync

                //Growing grass, vines, etc (manually)
                for (int x = CANOPY_X - CANOPY_H; x <= CANOPY_X + CANOPY_H; x++)
                {
                    for (int y = CANOPY_Y - CANOPY_V; y <= CANOPY_Y + CANOPY_V; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        for (int grassX = x - 1; grassX <= x + 1; grassX++)
                        {
                            for (int grassY = y - 1; grassY <= y + 1; grassY++)
                            {
                                Tile tile2 = Framing.GetTileSafely(grassX, grassY);
                                if (!tile2.HasTile)
                                {
                                    if (tile.TileType == TileID.LivingWood || TileID.Sets.Grass[tile.TileType])
                                        tile.TileType = (ushort)canopyGrass;

                                    if (tile.HasTile && tile2.WallType == 0)
                                        tile.WallType = 0;
                                }
                            }
                        }
                    }
                }
            }
            public static void GenerateLeaves(int TRUNK_X, int trunkY, int thickness)
            {
                float[] angles = new float[] { MathHelper.ToRadians(-120), MathHelper.ToRadians(-90), MathHelper.ToRadians(-45), MathHelper.ToRadians(45), MathHelper.ToRadians(90), MathHelper.ToRadians(120) };

                int numBranches = angles.Length;
                for (int i = 0; i < numBranches; i++)
                {
                    float angle = angles[i];
                    int branchLength = 32;

                    for (int j = 0; j < branchLength; j++)
                    {
                        int controlPointOffsetX = Main.rand.Next(-40, 40); // Random horizontal offset
                        int controlPointOffsetY = Main.rand.Next(-20, 0);  // Random vertical offset, upwards

                        float curve = (float)Math.Sin(j * 0.09f) * 3f;
                        int posX = TRUNK_X + (int)(j * Math.Cos(angle + curve));
                        int posY = trunkY - (int)(j * Math.Sin(angle + curve));

                        for (int tx = -thickness; tx <= thickness; tx++)
                        {
                            for (int ty = -thickness; ty <= thickness; ty++)
                            {
                                if (tx * tx + ty * ty <= thickness * thickness)
                                {
                                    WorldGen.TileRunner(posX + tx, posY + ty, 30, 30, TileID.LeafBlock, true, 1, 1);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}