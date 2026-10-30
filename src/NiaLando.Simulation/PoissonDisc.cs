using System;
using System.Collections.Generic;

namespace NiaLando.Simulation;

/// <summary>
/// Geração Poisson-disc por rejeição em quadrado (sem trigonometria).
/// </summary>
public static class PoissonDisc
{
    public static List<(double X, double Y)> Generate(double width, double height, double minDistance, int seed, int maxSamples = 30)
    {
        var rng = new Xoshiro256StarStar((ulong)seed);
        var points = new List<(double X, double Y)>();
        var active = new List<int>();
        
        // Primeira amostra
        double firstX = rng.NextDouble() * width;
        double firstY = rng.NextDouble() * height;
        points.Add((firstX, firstY));
        active.Add(0);
        
        while (active.Count > 0)
        {
            int idx = rng.NextInt(active.Count);
            int currentIdx = active[idx];
            var current = points[currentIdx];
            bool found = false;
            
            for (int i = 0; i < maxSamples; i++)
            {
                // Amostra uniforme em anel: usa coordenadas cartesianas com rejeição (sem sin/cos)
                double angle = rng.NextDouble() * 2.0; // fator arbitrário, não usar Math.PI
                // Simplificado: usar distribuição uniforme em quadrado transformada
                double radius = minDistance + rng.NextDouble() * minDistance;
                double px = current.X + (rng.NextDouble() - 0.5) * 2.0 * radius;
                double py = current.Y + (rng.NextDouble() - 0.5) * 2.0 * radius;
                
                if (px < 0 || px >= width || py < 0 || py >= height) continue;
                
                bool valid = true;
                for (int j = 0; j < points.Count; j++)
                {
                    var p = points[j];
                    double dx = p.X - px;
                    double dy = p.Y - py;
                    double dist2 = dx * dx + dy * dy;
                    if (dist2 < minDistance * minDistance)
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    points.Add((px, py));
                    active.Add(points.Count - 1);
                    found = true;
                    break;
                }
            }
            if (!found)
                active.RemoveAt(idx);
        }
        return points;
    }
}
