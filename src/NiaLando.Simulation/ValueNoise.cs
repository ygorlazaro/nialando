using System;

namespace NiaLando.Simulation;

/// <summary>
/// Ruído de valor (value noise) determinístico sem trigonometria.
/// Usa hash inteiro + interpolação polinomial (smoothstep).
/// </summary>
public static class ValueNoise
{
    public static ulong Hash(int x, int y, ulong seed)
    {
        ulong h = seed;
        h ^= (ulong)x * 131542391UL;
        h ^= (ulong)y * 1009UL;
        h = (h ^ (h >> 33)) * 0xff51afd7ed558ccdUL;
        h = (h ^ (h >> 33)) * 0xc4ceb9fe1a85ec53UL;
        return h ^ (h >> 33);
    }

    public static double Noise2D(double x, double y, ulong seed)
    {
        int x0 = (int)Math.Floor(x);
        int y0 = (int)Math.Floor(y);
        int x1 = x0 + 1;
        int y1 = y0 + 1;

        double sx = x - x0;
        double sy = y - y0;

        double n00 = HashToDouble(Hash(x0, y0, seed));
        double n10 = HashToDouble(Hash(x1, y0, seed));
        double n01 = HashToDouble(Hash(x0, y1, seed));
        double n11 = HashToDouble(Hash(x1, y1, seed));

        double ix0 = SmoothStep(n00, n10, sx);
        double ix1 = SmoothStep(n01, n11, sx);
        return SmoothStep(ix0, ix1, sy);
    }

    private static double HashToDouble(ulong h) => (h >> 11) * (1.0 / 9007199254740992.0);

    private static double SmoothStep(double a, double b, double t)
    {
        double tt = t * t * (3.0 - 2.0 * t);
        return a + (b - a) * tt;
    }
}
