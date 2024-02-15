using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using ReverieMod.Core.Mechanics;

namespace ReverieMod.Helpers
{
    /// <summary>
    /// Introduces new world generation tools such as Cellular Automata and shapes.
    /// </summary>
    /// <returns></returns>
    public class WorldGenHelpers
    {
        public static void GenerateBezierPath(BezierCurve curve, ushort tileType)
        {
            List<Vector2> points = curve.GetPoints(100); // Get 100 points along the curve

            foreach (Vector2 point in points)
            {
                WorldGen.PlaceTile((int)point.X, (int)point.Y, tileType, forced: true);
            }
        }
        /// <summary>
        /// Checks for tiles inside of a ovular radius.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="horizontalRadius"></param>
        /// <param name="verticalRadius"></param>
        /// <returns></returns>
        public static bool IsPointInsideEllipse(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius)
        {
            // The equation for an ellipse centered at (centerX, centerY) is:
            // ((x - centerX)^2 / horizontalRadius^2) + ((y - centerY)^2 / verticalRadius^2) <= 1
            // If the point (x, y) satisfies this inequality, it's inside the ellipse.

            float dx = (x - centerX);
            float dy = (y - centerY);
            return (dx * dx) / (horizontalRadius * horizontalRadius) + (dy * dy) / (verticalRadius * verticalRadius) <= 1;
        }
        /// <summary>
        ///  Checks if a tile is outside of an ovular radius. this should be used with 'IsPointInsideEllipse(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius)'.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="horizontalRadius"></param>
        /// <param name="verticalRadius"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool IsPointNearOvalEdge(int x, int y, int centerX, int centerY, int horizontalRadius, int verticalRadius, float threshold = 0.1f)
        {
            // Calculate the normalized distance from the point to the ellipse's edge
            float dx = (float)(x - centerX) / horizontalRadius;
            float dy = (float)(y - centerY) / verticalRadius;
            float distance = dx * dx + dy * dy;

            // Check if the point is near the edge of the ellipse
            return distance >= (1.0f - threshold) && distance <= (1.0f + threshold);
        }
        /// <summary>
        /// Generates Cellular Automata (for tiles) like in Connway's game of life. Set the density and iteration count for variance.
        /// parameters 'type' and 'forced' do not effect anything if 'killTile' is set to true but must still be called.
        /// </summary>
        /// <param name="cX"></param>
        /// <param name="cY"></param>
        /// <param name="hR"></param>
        /// <param name="vR"></param>
        /// <param name="density"></param>
        /// <param name="iterations"></param>
        /// <param name="kill"></param>
        /// <param name="type"></param>
        /// <param name="forced"></param>
        public static void GenerateCellularAutomata(int cX, int cY, int hR, int vR, int density, int iterations, bool killTile, int type, bool forced)
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
        /// <summary>
        /// Generates Cellular Automata (for walls) like in Connway's game of life. Set the density and iteration count for variance.
        /// </summary>
        /// <param name="cX"></param>
        /// <param name="cY"></param>
        /// <param name="hR"></param>
        /// <param name="vR"></param>
        /// <param name="density"></param>
        /// <param name="iterations"></param>
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
        private static bool IsClusterSolid(bool[,] map, int startX, int startY, int width, int height, int clusterSize)
        {
            for (int x = 0; x < clusterSize; x++)
            {
                for (int y = 0; y < clusterSize; y++)
                {
                    int checkX = startX + x;
                    int checkY = startY + y;

                    // Check if the tile is out of bounds or empty
                    if (checkX < 0 || checkX >= width || checkY < 0 || checkY >= height || !map[checkX, checkY])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
