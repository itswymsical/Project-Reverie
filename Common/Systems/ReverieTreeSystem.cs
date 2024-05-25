using Terraria.ModLoader;
using Terraria;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;
using ReverieMod.Content.Tiles.WoodlandCanopy;
using SteelSeries.GameSense;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.CodeDom;

namespace ReverieMod.Common.Systems
{
    public class ReverieTreeSystem : ModSystem
    {
        public static int treeWood = TileID.LivingWood;
        public static int treeWall = WallID.LivingWoodUnsafe;
        public static int canopyWall = WallID.FlowerUnsafe;
        public static int treeLeaves = TileID.LeafBlock;
        public static int canopyGrass = ModContent.TileType<WoodlandGrassTile>();

        public static bool InsideCanopyRadius(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius)
        {
            // The equation for an ellipse centered at (centerX, centerY) is:
            // ((x - centerX)^2 / horizontalRadius^2) + ((y - centerY)^2 / verticalRadius^2) <= 1
            // If the point (x, y) satisfies this inequality, it's inside the ellipse.

            float dx = (x - centerX);
            float dy = (y - centerY);
            return (dx * dx) / (horizontalRadius * horizontalRadius) + (dy * dy) / (verticalRadius * verticalRadius) <= 1;
        }
        public static bool OutsideRadius_Canopy(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius, float threshold = 0.1f)
        {
            float dx = (float)(x - centerX) / horizontalRadius;
            float dy = (float)(y - centerY) / verticalRadius;
            float distance = dx * dx + dy * dy;

            return distance >= (1.0f - threshold) && distance <= (1.0f + threshold);
        }
        public static void Gen_CaveNoiseMap(int cX, int cY, int hR, int vR, int density, int iterations, bool killTile, int type, bool forced)
        {
            bool[,] caveMap = new bool[hR * 2, vR * 2];
            for (int x = 0; x < hR * 2; x++)
            {
                for (int y = 0; y < vR * 2; y++)
                {
                    if (InsideCanopyRadius(x + cX - hR, y + cY - vR, cX, cY, hR, vR))
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
        public static void Gen_CaveNoiseMap_Wall(int cX, int cY, int hR, int vR, int density, int iterations)
        {
            bool[,] caveMap = new bool[hR * 2, vR * 2];
            for (int x = 0; x < hR * 2; x++)
            {
                for (int y = 0; y < vR * 2; y++)
                {
                    if (InsideCanopyRadius(x + cX - hR, y + cY - vR, cX, cY, hR, vR))
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
        public static Vector2 CalculatePoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector2 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        } //bezier >:)
        public static void GenRoot(Vector2 p0, Vector2 p1, Vector2 p2, ushort tileType)
        {
            for (float t = 0; t <= 1; t += 0.01f)
            {
                Vector2 point = CalculatePoint(t, p0, p1, p2);
                WorldGen.PlaceTile((int)point.X, (int)point.Y, tileType, mute: true, forced: true);
            }
        }
        
        public static void CarveRoot(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            for (float t = 0; t <= 1; t += 0.01f)
            {
                Vector2 point = CalculatePoint(t, p0, p1, p2);
                WorldGen.KillTile((int)point.X, (int)point.Y);
            }
        }
        public class TrunkPass : GenPass
        {
            public TrunkPass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Growing the Woodland Canopy";
                int TRUNK_X;
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
                int TRUNK_BOTTOM = (int)(Main.rockLayer + (Main.maxTilesY - Main.maxTilesY) / 16);

                int CANOPY_CENTER_X = TRUNK_X;
                int CANOPY_CENTER_Y = TRUNK_BOTTOM;

                int CANOPY_RADIUS_H = (int)(Main.maxTilesX * 0.032f);
                int CANOPY_RADIUS_V = (int)(Main.maxTilesY * 0.127f);

                TRUNK_X = Math.Clamp(TRUNK_X, 0, Main.maxTilesX - 1); //safety

                for (int x = CANOPY_CENTER_X - CANOPY_RADIUS_H; x <= CANOPY_CENTER_X + CANOPY_RADIUS_H; x++)
                {
                    for (int y = CANOPY_CENTER_Y - CANOPY_RADIUS_V; y <= CANOPY_CENTER_Y + CANOPY_RADIUS_V; y++)
                    {
                        progress.Set(x / (float)((float)(CANOPY_CENTER_X * 1.1f) - 1)); // Controls the progress bar, should only be set between 0f and 1f
           
                        if (InsideCanopyRadius(x, y, CANOPY_CENTER_X, CANOPY_CENTER_Y, CANOPY_RADIUS_H, CANOPY_RADIUS_V))
                        {
                            WorldGen.KillWall(x, y);
                            WorldGen.PlaceWall(x, y, canopyWall);
                            WorldGen.PlaceTile(x, y, 0, forced: true);
                        }

                        else if (OutsideRadius_Canopy(x, y, CANOPY_CENTER_X, CANOPY_CENTER_Y, CANOPY_RADIUS_H, CANOPY_RADIUS_V))
                        {
                            if (Main.rand.NextFloat() < 0.8f)
                            {
                                int border = Main.rand.Next(21, 30);
                                for (int i = 0; i < border; i++)
                                {
                                    int borderX = x + Main.rand.Next(-1, 2);
                                    int borderY = y + Main.rand.Next(-1, 2);
                                    if (!WorldGen.TileEmpty(borderX, borderY))
                                    {
                                        WorldGen.PlaceTile(borderX, borderY, 0, forced: true);
                                    }
                                }
                            }
                        }
                    }
                }

                int cellX = CANOPY_RADIUS_H - (CANOPY_RADIUS_H / 48);
                int cellY = CANOPY_RADIUS_V - (CANOPY_RADIUS_V / 48);

                Gen_CaveNoiseMap(CANOPY_CENTER_X, CANOPY_CENTER_Y, cellX, cellY, 50, 9, true, 0, false);
                Gen_CaveNoiseMap_Wall(CANOPY_CENTER_X, CANOPY_CENTER_Y, cellX, cellY, 50, 8);

                        GenerateLeaves(TRUNK_X, TRUNK_TOP, 4);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, TRUNK_X, TRUNK_TOP, TRUNK_WIDTH, TRUNK_BOTTOM - TRUNK_TOP + 1);
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

                        Vector2 start = new Vector2(TRUNK_X, trunkY);
                        Vector2 control = new Vector2(TRUNK_X + controlPointOffsetX, trunkY + controlPointOffsetY);
                        Vector2 end = new Vector2(TRUNK_X + controlPointOffsetX, trunkY + 20);
                        //GenRoot(start, control, end, (ushort)treeWood);

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
        public class ReverieExtrasPass : GenPass
        {
            public ReverieExtrasPass(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Manifesting Reverie";

                int TRUNK_X;
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

                int TRUNK_WIDTH = 11;
                int TRUNK_TOP = (int)(SPAWN_Y - (Main.maxTilesY - SPAWN_Y) / 14);
                int TRUNK_BOTTOM = (int)(Main.rockLayer - (Main.maxTilesY - Main.rockLayer) / 16);

                int CANOPY_CENTER_X = TRUNK_X;
                int CANOPY_CENTER_Y = TRUNK_BOTTOM;

                int CANOPY_RADIUS_H = (int)(Main.maxTilesX * 0.045f);
                int CANOPY_RADIUS_V = (int)(Main.maxTilesY * 0.125f);

                const float TRUNK_CURVE_FREQUENCY = 0.0765f;
                const int TRUNK_CURVE_AMPLITUDE = 4;

                TRUNK_X = Math.Clamp(TRUNK_X, 0, Main.maxTilesX - 1); //safety


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

                int topSectionEndY = CANOPY_CENTER_Y - CANOPY_RADIUS_V / 3;

                int shrineCANOPY_CENTER_X = TRUNK_X;
                int shrineCANOPY_CENTER_Y = topSectionEndY;

                int shrineCANOPY_RADIUS_H = (int)(Main.maxTilesX * 0.0095f);
                int shrineCANOPY_RADIUS_V = (int)(Main.maxTilesY * 0.0275f);

                int domeRadius = (int)(Main.maxTilesX * 0.0095f);
                
                for (int x = CANOPY_CENTER_X - CANOPY_RADIUS_H; x <= CANOPY_CENTER_X + CANOPY_RADIUS_H; x++)
                {
                    for (int y = CANOPY_CENTER_Y - CANOPY_RADIUS_V; y <= CANOPY_CENTER_Y + CANOPY_RADIUS_V; y++)
                    {

                        if (InsideCanopyRadius(x, y, CANOPY_CENTER_X, CANOPY_CENTER_Y, CANOPY_RADIUS_H, CANOPY_RADIUS_V))
                        {
                            Tile tile = Framing.GetTileSafely(x, y);
                            Tile tileBelow = Framing.GetTileSafely(x, y + 1);
                            Tile tileAbove = Framing.GetTileSafely(x, y - 1);
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
                                        WorldGen.SquareTileFrame(x, y + 1, true);
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
                
                for (int x = shrineCANOPY_CENTER_X - shrineCANOPY_RADIUS_H; x <= shrineCANOPY_CENTER_X + shrineCANOPY_RADIUS_H; x++)
                {
                    for (int y = shrineCANOPY_CENTER_Y - shrineCANOPY_RADIUS_V; y <= shrineCANOPY_CENTER_Y + shrineCANOPY_RADIUS_V; y++)
                    {
                        if (InsideCanopyRadius(x, y, shrineCANOPY_CENTER_X, shrineCANOPY_CENTER_Y, shrineCANOPY_RADIUS_H, shrineCANOPY_RADIUS_V))
                        {
                            WorldGen.PlaceTile(x, y, TileID.LivingWood, forced: true);
                            WorldGen.KillWall(x, y);
                        }
                    }
                }


                for (int x = shrineCANOPY_CENTER_X - domeRadius; x <= shrineCANOPY_CENTER_X + domeRadius; x++)
                {
                    for (int y = shrineCANOPY_CENTER_Y - domeRadius; y <= shrineCANOPY_CENTER_Y; y++) // Only go up to the midpoint for a dome shape
                    {
                        if (InsideCanopyRadius(x, y, shrineCANOPY_CENTER_X, shrineCANOPY_CENTER_Y, domeRadius, domeRadius))
                        {
                            WorldGen.KillTile(x, y + 15);
                            WorldGen.PlaceWall(x, y + 15, WallID.LivingWoodUnsafe);
                        }
                    }
                }
            }
        }
    }
}