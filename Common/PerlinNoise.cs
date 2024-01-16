using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trelamium.Common
{
    public static class PerlinNoise
    {
        private static int[] permutation = {
    180, 159, 198, 107, 37, 124, 88, 38, 100, 31, 242, 227, 65, 185, 221, 202,
    170, 194, 151, 135, 35, 14, 20, 27, 236, 172, 165, 44, 162, 183, 141, 160,
    98, 229, 63, 146, 22, 250, 17, 153, 96, 219, 80, 188, 212, 50, 161, 26,
    40, 140, 41, 230, 252, 210, 247, 226, 120, 222, 52, 15, 193, 190, 255, 64,
    175, 36, 199, 112, 131, 164, 69, 110, 122, 10, 8, 28, 29, 87, 216, 186,
    7, 46, 218, 139, 213, 134, 143, 24, 239, 71, 99, 133, 245, 95, 207, 223,
    182, 119, 125, 118, 66, 90, 76, 142, 235, 237, 5, 167, 244, 217, 192, 184,
    177, 126, 238, 191, 49, 54, 75, 251, 11, 42, 145, 136, 169, 70, 215, 123,
    154, 73, 33, 128, 67, 106, 19, 220, 179, 196, 102, 82, 68, 109, 228, 163,
    104, 129, 201, 48, 195, 78, 205, 25, 203, 211, 171, 3, 225, 137, 53, 2,
    181, 241, 58, 173, 39, 166, 94, 158, 243, 178, 246, 83, 117, 86, 144, 115,
    55, 234, 84, 111, 61, 114, 206, 0, 18, 60, 9, 176, 16, 187, 233, 105,
    156, 132, 121, 74, 189, 200, 231, 79, 174, 92, 32, 30, 51, 85, 23, 147,
    4, 249, 47, 209, 155, 248, 45, 113, 240, 103, 93, 43, 152, 149, 101, 130,
    89, 6, 91, 77, 12, 254, 108, 13, 34, 168, 208, 72, 56, 138, 232, 150,
    116, 224, 197, 127, 21, 59, 57, 157, 97, 62, 214, 148, 1, 253, 204, 81 };
        private static int[] p; // Doubled permutation to avoid overflow

        static PerlinNoise()
        {
            // Double the permutation array
            p = new int[512];
            for (int i = 0; i < 256; i++)
                p[256 + i] = p[i] = permutation[i];
        }

        public static double Generate(double x, double y, double z)
        {
            // Find unit cube that contains point
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;
            int Z = (int)Math.Floor(z) & 255;
            // Find relative x, y, z of point in cube
            x -= Math.Floor(x);
            y -= Math.Floor(y);
            z -= Math.Floor(z);
            // Compute fade curves for each of x, y, z
            double u = Fade(x);
            double v = Fade(y);
            double w = Fade(z);
            // Hash coordinates of the 8 cube corners
            int A = p[X] + Y;
            int AA = p[A] + Z;
            int AB = p[A + 1] + Z;
            int B = p[X + 1] + Y;
            int BA = p[B] + Z;
            int BB = p[B + 1] + Z;

            // Add blended results from 8 corners of cube
            double res = Lerp(w, Lerp(v, Lerp(u, Grad(p[AA], x, y, z),
                                               Grad(p[BA], x - 1, y, z)),
                                       Lerp(u, Grad(p[AB], x, y - 1, z),
                                               Grad(p[BB], x - 1, y - 1, z))),
                               Lerp(v, Lerp(u, Grad(p[AA + 1], x, y, z - 1),
                                               Grad(p[BA + 1], x - 1, y, z - 1)),
                                       Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1),
                                               Grad(p[BB + 1], x - 1, y - 1, z - 1))));
            return (res + 1.0) / 2.0;
        }
        private static double Fade(double t)
        {
            // Fade function as defined by Ken Perlin. This eases coordinate values
            // so that they will ease towards integral values. This smooths the final output.
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static double Grad(int hash, double x, double y, double z)
        {
            // Convert lower 4 bits of hash code into 12 gradient directions
            int h = hash & 15;
            double u = h < 8 ? x : y;
            double v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        private static double Lerp(double t, double a, double b)
        {
            // Linear interpolate between a and b
            return a + t * (b - a);
        }
    }
}
