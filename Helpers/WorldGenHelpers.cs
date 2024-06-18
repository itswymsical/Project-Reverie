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
    internal static partial class Helper
    {
        /// <summary>
        /// Generates a bezier path.
        /// 'i' represents the amount of points.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="tileType"></param>
        /// <param name="i"></param>
        public static void GenerateBezierPath(BezierCurve curve, ushort tileType, int i)
        {
            List<Vector2> points = curve.GetPoints(i); // Get 100 points along the curve

            foreach (Vector2 point in points)
            {
                WorldGen.PlaceTile((int)point.X, (int)point.Y, tileType, forced: true);
            }
        }
        /// <summary>
        /// Automatically slopes tiles by checking for empty neighbors via <seealso cref="WorldGen.TileIsExposedToAir(int, int)"/>.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void SmoothTerrain(int i, int j)
        {
            for (int x = 1; x < i - 1; x++)
            {
                for (int y = 1; y < j - 1; y++)
                {
                    if (WorldGen.TileIsExposedToAir(x, y))
                    {
                        Tile tile = Main.tile[x, y];

                        // Check left and right neighbors
                        bool hasLeftNeighbor = Main.tile[x - 1, y].HasTile;
                        bool hasRightNeighbor = Main.tile[x + 1, y].HasTile;
                        bool hasTopNeighbor = Main.tile[x, y - 1].HasTile;
                        bool hasBottomNeighbor = Main.tile[x, y + 1].HasTile;

                        if (hasTopNeighbor && hasRightNeighbor && !hasLeftNeighbor)
                        {
                            tile.Slope = SlopeType.SlopeUpLeft;
                        }
                        else if (hasTopNeighbor && !hasRightNeighbor && hasLeftNeighbor)
                        {
                            tile.Slope = SlopeType.SlopeUpRight;
                        }
                        else if (hasBottomNeighbor && hasLeftNeighbor && !hasRightNeighbor)
                        {
                            tile.Slope = SlopeType.SlopeDownLeft;
                        }
                        else if (hasBottomNeighbor && hasRightNeighbor && !hasLeftNeighbor)
                        {
                            tile.Slope = SlopeType.SlopeDownRight;
                        }
                        else if (hasLeftNeighbor && hasRightNeighbor)
                        {
                            tile.Slope = SlopeType.Solid;
                        }
                    }
                }
            }
        }
    }
}