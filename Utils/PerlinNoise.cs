using Microsoft.Xna.Framework;
using System;

namespace ReverieMod
{
    public class PerlinNoise
    {
        // The permutation array used for the noise function, duplicated to avoid overflow
        private static int[] _permutation;
        private Random _random;

        public int Seed { get; }

        public PerlinNoise(int seed)
        {
            _random = new Random(seed);
            Seed = seed;

            _permutation = new int[512];
            // Initialize and shuffle the array and duplicate it to avoid overflow
            for (int i = 0; i < 256; i++)
            {
                _permutation[i] = i;
            }
            for (int i = 0; i < 256; i++)
            {
                int j = _random.Next(256 - i) + i;
                int temp = _permutation[i];
                _permutation[i] = _permutation[j];
                _permutation[j] = temp;
                _permutation[i + 256] = _permutation[i];
            }
        }

        // Generates Perlin noise for a point (x, y)
        public static float Noise(float x, float y)
        {
            // Find unit cube that contains point
            int pX = (int)Math.Floor(x) & 255;
            int pY = (int)Math.Floor(y) & 255;

            // Find relative x, y of point in cube
            x -= (float)Math.Floor(x);
            y -= (float)Math.Floor(y);

            // Compute fade curves for each of x, y
            float u = Fade(x);
            float v = Fade(y);

            int pA = (_permutation[pX] + pY) & 255;
            int pB = (_permutation[pX + 1] + pY) & 255;

            //returns value from -1 to 1
            return MathHelper.Lerp(
                MathHelper.Lerp(Grad(_permutation[pA], x, y), Grad(_permutation[pB], x - 1, y), u),
                MathHelper.Lerp(Grad(_permutation[pA + 1], x, y - 1), Grad(_permutation[pB + 1], x - 1, y - 1), u),
                v);
        }

        // Public method to generate Perlin noise
        public static float Generate(float x, float y)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0; // Used for normalizing result to [0, 1]

            // Loop through octaves
            for (int i = 0; i < 4; i++)
            {
                total += Noise(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= 0.5f;
                frequency *= 2;
            }

            return total / maxValue; // Normalize the result
        }

        // Overloaded method to generate Perlin noise with specific frequency and amplitude
        public static float Generate(float x, float y, float frequency, float amplitude, float maxValue)
        {
            float total = 0;

            // Loop through octaves
            for (int i = 0; i < 4; i++)
            {
                total += Noise(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= 0.5f;
                frequency *= 2;
            }

            return total / maxValue; // Normalize the result
        }

        // Fade function to smooth the transitions
        private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

        // Gradient function calculates dot product between gradient vector and distance vector
        private static float Grad(int hash, float x, float y) => ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
    }
}