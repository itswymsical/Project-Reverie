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
    }
}
