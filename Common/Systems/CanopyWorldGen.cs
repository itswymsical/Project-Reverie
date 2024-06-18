using Microsoft.Xna.Framework;
using ReverieMod.Content.Tiles.Canopy;
using ReverieMod.Helpers;
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

        public static int SPAWN_X = Main.maxTilesX / 2;
        public static int SPAWN_Y = (int)Main.worldSurface;

        public static int TRUNK_BOTTOM = (SPAWN_Y + (SPAWN_Y / 2));

        public static int CANOPY_X = SPAWN_X;
        public static int CANOPY_Y = TRUNK_BOTTOM + (TRUNK_BOTTOM / 4);

        public static int CANOPY_H = (int)(Main.maxTilesX * 0.02475f);
        public static int CANOPY_V = (int)(Main.maxTilesY * 0.170f);

        public static int treeWood = TileID.LivingWood;
        public static int treeWall = WallID.LivingWoodUnsafe;
        public static int canopyWall = WallID.DirtUnsafe4;
        public static int canopyWall_Alt = WallID.DirtUnsafe1;
        public static int treeLeaves = TileID.LeafBlock;
        public static int canopyGrass = ModContent.TileType<Woodgrass>();
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
        // These "NoiseMap"'s are actually just Cellular Automata using Conway's rules.
        // Replaced with FastNoiseLite, but keeping for the wall design.
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
        public class CanopyPass : GenPass
        {
            public CanopyPass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Generating Woodland Canopy";
                int TRUNK_DIR = Main.rand.Next(2);
                int SPAWN_DISTANCE = (Main.maxTilesX - SPAWN_X) / 64;

                TRUNK_X = (TRUNK_DIR == 0) ? SPAWN_X - SPAWN_DISTANCE : SPAWN_X + SPAWN_DISTANCE;
                TRUNK_X = Math.Clamp(TRUNK_X, 0, Main.maxTilesX - 1); // Dunnno why this is here.

                int wallType = Main.rand.NextBool(2) ? canopyWall_Alt : canopyWall; // Swap the walls with an alternate version.

                // Setting the area for the canopy coordinates.
                for (int x = CANOPY_X - CANOPY_H; x <= CANOPY_X + CANOPY_H; x++)
                {
                    for (int y = CANOPY_Y - CANOPY_V; y <= CANOPY_Y + CANOPY_V; y++)
                    {                    
                        WorldGen.KillWall(x, y);
                        WorldGen.PlaceWall(x, y, wallType);
                        WorldGen.TileRunner(x, y, 20, 5, treeWood, true, 0, 0, false, true);

                        // This is the blue progress bar that tracks the actual speed of the generation.
                        progress.Set((float)((x - (CANOPY_X - CANOPY_H)) * (2 * CANOPY_V) + (y - (CANOPY_Y - CANOPY_V))) / ((2 * CANOPY_H) * (2 * CANOPY_V)));
                    }
                }
            }
        }
        public class CanopyRootPass : GenPass
        {
            public CanopyRootPass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Overgrowing tree roots";
                FastNoiseLite noise = new FastNoiseLite();
                noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
                noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
                noise.SetFractalGain(0.325f);
                noise.SetFractalOctaves(5); // Since this is fractal noise, our noise map is almost infinite.
                // '<' value zooms out of the noise grid (many, small caves), '>' value zooms in (bigger caves)
                noise.SetFrequency(0.032f); //i think this a good value.
                int posx = CANOPY_H * 2;
                int posy = CANOPY_V * 2;
                float[,] noiseData = new float[posx, posy];
                float threshold = 0.42f; // This is basically which noise values are we going to ignore/kill.
                // Not entirely sure, but i believe positive values calculate the bright values.

                // Locating canopy & making noise map.
                for (int x = 0; x < posx; x++)
                {
                    for (int y = 0; y < posy; y++)
                    {
                        int worldX = x + (CANOPY_X - CANOPY_H);
                        int worldY = y + (CANOPY_Y - CANOPY_V);

                        noiseData[x, y] = noise.GetNoise(worldX, worldY);
                        Helper.SmoothTerrain(worldX, worldY);
                    }
                }

                // Iterates through Canopy Coordinates, kills tiles under a specific threshold value.
                for (int x = 0; x < posx; x++)
                {
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
                
                Gen_NoiseMap_Walls(CANOPY_X, CANOPY_Y, CANOPY_H, CANOPY_V, 48, 10);
            }
        }
        public class ReverieTreePass : GenPass
        {
            public ReverieTreePass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Weathering tree roots";
                int TRUNK_DIR = Main.rand.Next(2);
                int SPAWN_DISTANCE = (Main.maxTilesX - SPAWN_X) / 64;

                TRUNK_X = (TRUNK_DIR == 0) ? SPAWN_X - SPAWN_DISTANCE : SPAWN_X + SPAWN_DISTANCE;
                TRUNK_X = Math.Clamp(TRUNK_X, 0, Main.maxTilesX - 1);

                int TRUNK_WIDTH = 12;
                int TRUNK_TOP = (SPAWN_Y - (SPAWN_Y / 3));

                // Sine wave shit for the tree.
                const float TRUNK_CURVE_FREQUENCY = 0.0765f;
                const int TRUNK_CURVE_AMPLITUDE = 4;

                // Placing trunk wood.
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

                // Placing trunk wall.
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

                // The tree top of Reverie.
                GenerateLeaves(TRUNK_X, TRUNK_TOP, 2);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, TRUNK_X, TRUNK_TOP, TRUNK_WIDTH, TRUNK_BOTTOM - TRUNK_TOP + 1); // Some netcode shit, I don't know.

                // Growing woodgrass on the wood blocks.
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
                                if (!tile2.HasTile) // Places less grass because yucky.
                                {
                                    if (tile.TileType == TileID.LivingWood || TileID.Sets.Grass[tile.TileType])
                                        tile.TileType = (ushort)canopyGrass;

                                    if (tile.HasTile && tile2.WallType == 0)
                                        tile.WallType = 0;
                                }
                            }
                        }
                        Tile tileAbove = Framing.GetTileSafely(x, y - 1);
                        if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                        {
                            WorldGen.PlaceTile(x, y - 1, (ushort)ModContent.TileType<CanopyFoliage>());
                            tileAbove.TileFrameY = 0;
                            tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(10) * 18);
                            WorldGen.SquareTileFrame(x, y + 1, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, x, y - 1, 1, TileChangeType.None);
                            }
                        }
                        Tile tileBelow = Framing.GetTileSafely(x, y + 1);
                        if (WorldGen.genRand.NextBool(3) && !tileBelow.HasTile && !tileBelow.LeftSlope && !tileBelow.RightSlope && !tileBelow.IsHalfBlock)
                        {
                            if (!tile.BottomSlope)
                            {
                                tileBelow.TileType = (ushort)ModContent.TileType<CanopyVine>();
                                tileBelow.HasTile = true;
                                WorldGen.SquareTileFrame(x, y + 1, true);
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendTileSquare(-1, x, y + 1, 3, 0);
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
                                    WorldGen.TileRunner(posX + tx, posY + ty, 19, 27, TileID.LeafBlock, true, 1, 1);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}