using Microsoft.Xna.Framework;
using ReverieMod.Content.Tiles.WoodlandCanopy;
using System;
using Terraria;
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
        public static int canopyWall = WallID.FlowerUnsafe;
        public static int treeLeaves = TileID.LeafBlock;
        public static int canopyGrass = ModContent.TileType<WoodlandGrassTile>();
        public static int canopyVines = ModContent.TileType<CanopyVine>();


        public static bool InsideCanopy(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius)
        {
            // This is the interior/base shape that generates prior to each other pass. We've created an elipse shape.
            // ((x - centerX)^2 / horizontalRadius^2) + ((y - centerY)^2 / verticalRadius^2) <= 1
            // If the point (x, y) satisfies this inequality, it's inside the ellipse.

            float dx = (x - centerX);
            float dy = (y - centerY);
            return (dx * dx) / (horizontalRadius * horizontalRadius) + (dy * dy) / (verticalRadius * verticalRadius) <= 1;
        }
        public static bool OutsideCanopy(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius, float threshold = 0.1f)
        {
            float dx = (float)(x - centerX) / horizontalRadius;
            float dy = (float)(y - centerY) / verticalRadius;
            float distance = dx * dx + dy * dy;

            return distance >= (1.0f - threshold) && distance <= (1.0f + threshold);
        }
        public static void Gen_NoiseMap(int cX, int cY, int hR, int vR, int density, int iterations, bool killTile, int type, bool forced)
        {
            bool[,] caveMap = new bool[hR * 2, vR * 2];
            for (int x = 0; x < hR * 2; x++)
            {
                for (int y = 0; y < vR * 2; y++)
                {
                    if (InsideCanopy(x + cX - hR, y + cY - vR, cX, cY, hR, vR))
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
                    if (InsideCanopy(x + cX - hR, y + cY - vR, cX, cY, hR, vR))
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
                progress.Message = "Foresting caverns";
                #region VARIABLES
                int TRUNK_DIR = Main.rand.Next(2);

                int SPAWN_X = Main.maxTilesX / 2;
                int SPAWN_Y = (int)Main.worldSurface - (Main.maxTilesY / 12);
                int SPAWN_DISTANCE = (Main.maxTilesX - SPAWN_X) / 20;
                if (TRUNK_DIR == 0)
                {
                    TRUNK_X = SPAWN_X - SPAWN_DISTANCE;
                }
                else
                {
                    TRUNK_X = SPAWN_X + SPAWN_DISTANCE;
                }

                int TRUNK_WIDTH = 13;
                int TRUNK_TOP = (int)(SPAWN_Y - (Main.maxTilesY - SPAWN_Y) / 14);
                int TRUNK_BOTTOM = (int)(Main.worldSurface + (Main.maxTilesY - Main.maxTilesY) / 12);

                int CANOPY_X = TRUNK_X;
                int CANOPY_Y = TRUNK_BOTTOM + (TRUNK_BOTTOM / 2);

                //Horizontal and Vertical Radius of the Elipse
                int CANOPY_H = (int)(Main.maxTilesX * 0.035f);
                int CANOPY_V = (int)(Main.maxTilesY * 0.128f);

                TRUNK_X = Math.Clamp(TRUNK_X, 0, Main.maxTilesX - 1); //safety
                #endregion

                for (int x = CANOPY_X - CANOPY_H; x <= CANOPY_X + CANOPY_H; x++)
                {
                    for (int y = CANOPY_Y - CANOPY_V; y <= CANOPY_Y + CANOPY_V; y++)
                    {
                        progress.Set(x / (float)((float)(CANOPY_X * 1.1f) - 1)); 
                        // Controls the progress bar, should only be set between 0f and 1f
                        // TODO: make the progress bar accurately calculate the the percentage from the first block generated to the last within the elipse.

                        if (InsideCanopy(x, y, CANOPY_X, CANOPY_Y, CANOPY_H, CANOPY_V))
                        {
                            WorldGen.KillWall(x, y);
                            WorldGen.PlaceWall(x, y, canopyWall);
                            WorldGen.PlaceTile(x, y, 0, forced: true);
                        }

                        else if (OutsideCanopy(x, y, CANOPY_X, CANOPY_Y, CANOPY_H, CANOPY_V))
                        {
                            if (Main.rand.NextFloat() < 0.8f)
                            {
                                int border = Main.rand.Next(21, 30);
                                for (int i = 0; i < border; i++)
                                {
                                    int borderX = x + Main.rand.Next(-2, 4);
                                    int borderY = y + Main.rand.Next(-2, 4);
                                    if (!WorldGen.TileEmpty(borderX, borderY))
                                    {
                                        WorldGen.PlaceTile(borderX + Main.rand.Next(-2, 4), borderY + Main.rand.Next(-2, 4), 0, forced: true); //this is a blend effect emulating noise around the biome
                                    }
                                }
                            }
                        }
                    }
                }

                Gen_NoiseMap(CANOPY_X, CANOPY_Y, CANOPY_H - (CANOPY_H / 56), CANOPY_V - (CANOPY_V / 48), 50, 9, true, 0, false);
                Gen_NoiseMap_Walls(CANOPY_X, CANOPY_Y, CANOPY_H - (CANOPY_H / 56), CANOPY_V - (CANOPY_V / 48), 50, 8);

                GenerateLeaves(TRUNK_X, TRUNK_TOP, 4);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, TRUNK_X, TRUNK_TOP, TRUNK_WIDTH, TRUNK_BOTTOM - TRUNK_TOP + 1);
            }
            //TODO: Rewrite the leaves with the new fractal tree
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
                int SPAWN_Y = (int)Main.worldSurface - (Main.maxTilesY / 12);
                int SPAWN_DISTANCE = (Main.maxTilesX - SPAWN_X) / 20;
                if (TRUNK_DIR == 0)
                {
                    TRUNK_X = SPAWN_X - SPAWN_DISTANCE;
                }
                else
                {
                    TRUNK_X = SPAWN_X + SPAWN_DISTANCE;
                }

                int TRUNK_WIDTH = 13;
                int TRUNK_TOP = (int)(SPAWN_Y - (Main.maxTilesY - SPAWN_Y) / 14);
                int TRUNK_BOTTOM = (int)(Main.worldSurface + (Main.maxTilesY - Main.maxTilesY) / 12);

                int CANOPY_X = TRUNK_X;
                int CANOPY_Y = TRUNK_BOTTOM + (TRUNK_BOTTOM / 2);

                //Horizontal and Vertical Radius of the Elipse
                int CANOPY_H = (int)(Main.maxTilesX * 0.035f);
                int CANOPY_V = (int)(Main.maxTilesY * 0.128f);

                TRUNK_X = Math.Clamp(TRUNK_X, 0, Main.maxTilesX - 1); //safety
                #endregion

                const float TRUNK_CURVE_FREQUENCY = 0.0765f;
                const int TRUNK_CURVE_AMPLITUDE = 4;

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
                //Growing grass, vines, etc (manually)
                for (int x = CANOPY_X - CANOPY_H; x <= CANOPY_X + CANOPY_H; x++)
                {
                    for (int y = CANOPY_Y - CANOPY_V; y <= CANOPY_Y + CANOPY_V; y++)
                    {

                        if (InsideCanopy(x, y, CANOPY_X, CANOPY_Y, CANOPY_H, CANOPY_V))
                        {
                            Tile tile = Framing.GetTileSafely(x, y);
                            Tile tileBelow = Framing.GetTileSafely(x, y + 1);
                            Tile tileAbove = Framing.GetTileSafely(x, y - 1);
                            if (!tileAbove.HasTile && !(tileAbove.LiquidType == LiquidID.Lava) && !(tileAbove.LiquidType == LiquidID.Water))
                            {
                                if (!tile.BottomSlope)
                                {
                                    tileAbove.TileType = (ushort)ModContent.TileType<AlderwoodSapling>();
                                    tileAbove.HasTile = true;
                                    WorldGen.SquareTileFrame(x, y - 1, true);
                                    if (Main.netMode == NetmodeID.Server)
                                    {
                                        NetMessage.SendTileSquare(-1, x, y - 1, 1, 0);
                                    }
                                }
                            }
                            for (int grassX = x - 1; grassX <= x + 1; grassX++)
                            {
                                if (!tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava) && !(tileBelow.LiquidType == LiquidID.Water))
                                {
                                    if (!tile.BottomSlope)
                                    {
                                        tileBelow.TileType = (ushort)ModContent.TileType<CanopyVine>();
                                        tileBelow.HasTile = true;
                                        WorldGen.SquareTileFrame(x, y + 1, true);
                                        if (Main.netMode == NetmodeID.Server)
                                        {
                                            NetMessage.SendTileSquare(-1, x, y + 1, 1, 0);
                                        }
                                    }
                                }
                                if (!tileAbove.HasTile && !(tileAbove.LiquidType == LiquidID.Lava) && !(tileAbove.LiquidType == LiquidID.Water))
                                {
                                    if (!tile.BottomSlope)
                                    {
                                        tileAbove.TileType = (ushort)ModContent.TileType<CanopyGrassFoliageTile>();
                                        tileAbove.HasTile = true;
                                        WorldGen.SquareTileFrame(x, y - 1, true);
                                        if (Main.netMode == NetmodeID.Server)
                                        {
                                            NetMessage.SendTileSquare(-1, x, y - 1, 1, 0);
                                        }
                                    }
                                }
                                for (int grassY = y - 1; grassY <= y + 1; grassY++)
                                {
                                    Tile tile2 = Framing.GetTileSafely(grassX, grassY);
                                    if (!tile2.HasTile)
                                    {
                                        if (tile.TileType == TileID.Dirt || TileID.Sets.Grass[tile.TileType])
                                            tile.TileType = (ushort)canopyGrass;

                                        if (tile.HasTile && tile2.WallType == 0)
                                            tile.WallType = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}