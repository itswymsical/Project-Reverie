using Microsoft.Xna.Framework;
using System;

public class PerlinNoise
{
    private readonly int[] permutation = new int[512];
    private const int Repeat = 256;

    public PerlinNoise(int seed)
    {
        var random = new Random(seed);
        for (int i = 0; i < 256; i++)
        {
            permutation[i] = i;
        }

        for (int i = 0; i < 256; i++)
        {
            int j = random.Next(256 - i) + i;
            int temp = permutation[i];
            permutation[i] = permutation[j];
            permutation[j] = temp;
            permutation[i + 256] = permutation[i];
        }
    }

    private float Noise(float x, float y, float z)
    {
        int xi = (int)Math.Floor(x) & 255;
        int yi = (int)Math.Floor(y) & 255;
        int zi = (int)Math.Floor(z) & 255;

        x -= (float)Math.Floor(x);
        y -= (float)Math.Floor(y);
        z -= (float)Math.Floor(z);

        float u = Fade(x);
        float v = Fade(y);
        float w = Fade(z);

        int A = permutation[xi] + yi;
        int AA = permutation[A] + zi;
        int AB = permutation[A + 1] + zi;
        int B = permutation[xi + 1] + yi;
        int BA = permutation[B] + zi;
        int BB = permutation[B + 1] + zi;

        return MathHelper.Lerp(w, MathHelper.Lerp(v, MathHelper.Lerp(u, Grad(permutation[AA], x, y, z), Grad(permutation[BA], x - 1, y, z)),
                               MathHelper.Lerp(u, Grad(permutation[AB], x, y - 1, z), Grad(permutation[BB], x - 1, y - 1, z))),
                               MathHelper.Lerp(v, MathHelper.Lerp(u, Grad(permutation[AA + 1], x, y, z - 1), Grad(permutation[BA + 1], x - 1, y, z - 1)),
                               MathHelper.Lerp(u, Grad(permutation[AB + 1], x, y - 1, z - 1), Grad(permutation[BB + 1], x - 1, y - 1, z - 1))));
    }

    public float Generate(float x, float y, float z)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < 4; i++)
        {
            total += Noise(x * frequency, y * frequency, z * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= 0.5f;
            frequency *= 2;
        }

        return total / maxValue;
    }

    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private float Grad(int hash, float x, float y, float z)
    {
        int h = hash & 15;
        float u = h < 8 ? x : y;
        float v = h < 4 ? y : h == 12 || h == 14 ? x : z; // Improved readability
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}