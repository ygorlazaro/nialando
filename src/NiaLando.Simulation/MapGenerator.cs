using System;
using System.Collections.Generic;
using System.Linq;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class MapGenerator
{
    public GameState Generate(MapParams mapParams)
    {
        int width = mapParams.Width > 0 ? mapParams.Width : 512;
        int height = mapParams.Height > 0 ? mapParams.Height : 512;
        double areaW = mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000.0;
        double areaH = mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000.0;
        int target = mapParams.NumProvinciasAlvo > 0 ? mapParams.NumProvinciasAlvo : 200;
        int lloydIter = mapParams.IteracoesLloyd >= 0 ? mapParams.IteracoesLloyd : 3;
        int octaves = mapParams.OctavasRuido > 0 ? mapParams.OctavasRuido : 5;
        double targetLand = mapParams.PorcentagemTerra > 0 && mapParams.PorcentagemTerra < 1 ? mapParams.PorcentagemTerra : 0.45;
        long seed = mapParams.Seed;

        var state = new GameState
        {
            Seed = seed,
            Params = mapParams,
            GeneratorVersion = GeneratorVersion.Current,
            SaveVersion = 1,
            CurrentTurn = 0,
            CurrentYear = 1444,
            Map = new MapData
            {
                Width = width,
                Height = height,
                GeneratorVersion = GeneratorVersion.Current,
                ProvinceIdRaster = new ushort[width * height]
            }
        };

        // 1. Poisson-disc com TODOS os pontos; ajusta a distância mínima para
        //    atingir o número alvo (não corta os primeiros N).
        double minDist = mapParams.DistanciaMinima > 0 ? mapParams.DistanciaMinima : 33.33;
        var points = GeneratePoissonTargetCount(areaW, areaH, target, minDist, seed);

        // 2. Relaxamento de Lloyd (aplicado de fato).
        points = LloydRelax(points, areaW, areaH, width, height, lloydIter);

        // 3. Relevo com ValueNoise fractal em coordenadas normalizadas + umidade.
        double[] heightMap = new double[width * height];
        double[] humidityMap = new double[width * height];
        ulong noiseSeed = (ulong)seed ^ 0x123456789ABCDEFUL;
        for (int y = 0; y < height; y++)
        {
            double ny = y / (double)height;
            for (int x = 0; x < width; x++)
            {
                double nx = x / (double)width;
                int idx = y * width + x;
                heightMap[idx] = FractalValueNoise(nx, ny, octaves, noiseSeed);
                humidityMap[idx] = FractalValueNoise(nx * 1.7 + 0.3, ny * 1.3 + 0.9, octaves, noiseSeed ^ 0x9E3779B9UL);
            }
        }

        // Limiar por quantil para atingir PorcentagemTerra.
        var sorted = heightMap.OrderBy(v => v).ToArray();
        int landCountTarget = (int)Math.Round(width * height * targetLand);
        landCountTarget = Math.Clamp(landCountTarget, (int)(width * height * 0.1), (int)(width * height * 0.9));
        double threshold = sorted[Math.Clamp(sorted.Length - landCountTarget, 0, sorted.Length - 1)];
        bool[] isLand = new bool[heightMap.Length];
        for (int i = 0; i < heightMap.Length; i++)
            isLand[i] = heightMap[i] >= threshold;

        // 4. Raster via grade espacial (não O(pixels × sites)).
        int[] nearest = AssignNearest(points, width, height, areaW, areaH);
        var raster = new ushort[width * height];
        for (int i = 0; i < raster.Length; i++)
            raster[i] = isLand[i] ? (ushort)(nearest[i] + 1) : (ushort)0;
        state.Map.ProvinceIdRaster = raster;

        // 5. Contagem terrestre por província em um único passe (O(pixels)).
        int n = points.Count;
        int[] landCount = new int[n + 1]; // indexado por id de província (1-based)
        long[] sumX = new long[n + 1];
        long[] sumY = new long[n + 1];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int pidx = y * width + x;
                int provId = raster[pidx];
                if (provId != 0 && isLand[pidx])
                {
                    landCount[provId]++;
                    sumX[provId] += x;
                    sumY[provId] += y;
                }
            }
        }

        // Garante que toda província tenha pelo menos 1 pixel terrestre (repção).
        for (int s = 0; s < n; s++)
        {
            int provId = s + 1;
            if (landCount[provId] > 0) continue;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pidx = y * width + x;
                    if (nearest[pidx] == s && !isLand[pidx])
                    {
                        raster[pidx] = (ushort)provId;
                        isLand[pidx] = true;
                        state.Map.ProvinceIdRaster[pidx] = (ushort)provId;
                        landCount[provId] = 1;
                        sumX[provId] = x;
                        sumY[provId] = y;
                        goto nextProvince;
                    }
                }
            }
            nextProvince:;
        }

        // Repara centróide: o pixel central de cada província deve ser dela.
        for (int provId = 1; provId <= n; provId++)
        {
            if (landCount[provId] == 0) continue;
            int cx = Math.Clamp((int)Math.Round((double)sumX[provId] / landCount[provId]), 0, width - 1);
            int cy = Math.Clamp((int)Math.Round((double)sumY[provId] / landCount[provId]), 0, height - 1);
            int pidx = cy * width + cx;
            if (raster[pidx] == 0)
            {
                raster[pidx] = (ushort)provId;
                isLand[pidx] = true;
                state.Map.ProvinceIdRaster[pidx] = (ushort)provId;
                landCount[provId]++;
                sumX[provId] += cx;
                sumY[provId] += cy;
            }
        }

        // Vizinha de água por província em um único passe (O(pixels)).
        bool[] hasWaterNeighbor = new bool[n + 1];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int pidx = y * width + x;
                int provId = raster[pidx];
                if (provId == 0) continue;
                if ((x > 0 && raster[pidx - 1] == 0) ||
                    (x < width - 1 && raster[pidx + 1] == 0) ||
                    (y > 0 && raster[pidx - width] == 0) ||
                    (y < height - 1 && raster[pidx + width] == 0))
                    hasWaterNeighbor[provId] = true;
            }
        }

        // 6. Províncias (todas as sites, mesmo as sem pixel terrestre).
        var provinceList = new List<Province>(points.Count);
        var rng = new Xoshiro256StarStar((ulong)seed ^ 0x5EEDUL);
        for (int i = 0; i < points.Count; i++)
            provinceList.Add(BuildProvince(i + 1, points[i], areaW, areaH, width, height, raster, isLand, heightMap, humidityMap, landCount, sumX, sumY, hasWaterNeighbor, rng));

        // 7. Portos (já definidos no construtor) e ajuste de terreno litoral.
        foreach (var prov in provinceList)
            if (prov.HasPort && prov.Terrain != Terrain.Mountains)
                prov.Terrain = Terrain.Coast;

        // 7. Forte inicial na província mais próxima do centro do mapa.
        int fortIdx = ProvinceClosestToCenter(provinceList, points, areaW, areaH);
        if (fortIdx >= 0) provinceList[fortIdx].FortLevel = 1;

        // 8. Adjacências derivadas do raster (geometria), sem sorteio.
        var adjSet = new Dictionary<int, HashSet<int>>();
        foreach (var prov in provinceList) adjSet[prov.Id.Value] = new HashSet<int>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                ushort cur = raster[idx];
                if (cur == 0) continue;
                if (x + 1 < width)
                {
                    ushort nb = raster[idx + 1];
                    if (nb != 0 && nb != cur) { adjSet[cur].Add(nb); adjSet[nb].Add(cur); }
                }
                if (y + 1 < height)
                {
                    ushort nb = raster[idx + width];
                    if (nb != 0 && nb != cur) { adjSet[cur].Add(nb); adjSet[nb].Add(cur); }
                }
            }
        }
        foreach (var prov in provinceList)
            prov.AdjacentProvinceIds = adjSet[prov.Id.Value].Select(v => new EntityId(v)).ToList();

        // 9. Zonas marítimas (componentes conexos de água, IDs distintos das províncias).
        var zoneData = BuildMaritimeZones(raster, width, height, points.Count + 1);
        foreach (var (z, cells) in zoneData)
        {
            var set = new HashSet<int>();
            foreach (var cell in cells)
            {
                int y = cell / width, x = cell % width;
                if (x > 0 && raster[y * width + x - 1] != 0) set.Add(raster[y * width + x - 1]);
                if (x < width - 1 && raster[y * width + x + 1] != 0) set.Add(raster[y * width + x + 1]);
                if (y > 0 && raster[(y - 1) * width + x] != 0) set.Add(raster[(y - 1) * width + x]);
                if (y < height - 1 && raster[(y + 1) * width + x] != 0) set.Add(raster[(y + 1) * width + x]);
            }
            z.AdjacentProvinceIds = set.Select(v => new EntityId(v)).ToList();
            z.HasPortAccess = true;
        }

        state.Map.Provinces = provinceList;
        state.Map.MaritimeZones = zoneData.Select(z => z.Zone).ToList();
        return state;
    }

    private static Province BuildProvince(int id, (double X, double Y) site, double areaW, double areaH,
        int width, int height, ushort[] raster, bool[] isLand, double[] heightMap, double[] humidityMap,
        int[] landCount, long[] sumX, long[] sumY, bool[] hasWaterNeighbor, Xoshiro256StarStar rng)
    {
        int landPixels = landCount[id];
        int cx, cy;
        double nx, ny;
        if (landPixels > 0)
        {
            cx = Math.Clamp((int)Math.Round((double)sumX[id] / landPixels), 0, width - 1);
            cy = Math.Clamp((int)Math.Round((double)sumY[id] / landPixels), 0, height - 1);
        }
        else
        {
            cx = Math.Clamp((int)Math.Round(site.X / areaW * (width - 1)), 0, width - 1);
            cy = Math.Clamp((int)Math.Round(site.Y / areaH * (height - 1)), 0, height - 1);
        }
        nx = site.X / areaW;
        ny = site.Y / areaH;

        int cellIdx = cy * width + cx;
        double h = heightMap[cellIdx];
        double m = humidityMap[cellIdx];
        bool coast = hasWaterNeighbor[id];
        var terrain = ClassifyTerrain(h, m, coast);
        var climate = ClassifyClimate(nx, ny, h, m);
        int dev = 5 + TerrainBonus(terrain) + rng.NextInt(3) - 1;
        dev = Math.Clamp(dev, 1, 10);

        return new Province
        {
            Id = new EntityId(id),
            Name = $"Prov{id}",
            Terrain = terrain,
            Climate = climate,
            Development = dev,
            TradeValue = 1 + dev / 5,
            HasPort = coast,
            FortLevel = 0,
            AdjacentProvinceIds = new List<EntityId>(),
            Populations = new List<ProvincePopulation>()
        };
    }

    private static bool HasWaterNeighbor(ushort provId, ushort[] raster, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (raster[y * width + x] != provId) continue;
                if ((x > 0 && raster[y * width + x - 1] == 0) ||
                    (x < width - 1 && raster[y * width + x + 1] == 0) ||
                    (y > 0 && raster[(y - 1) * width + x] == 0) ||
                    (y < height - 1 && raster[(y + 1) * width + x] == 0))
                    return true;
            }
        }
        return false;
    }

    private static int ProvinceClosestToCenter(List<Province> provinces, List<(double X, double Y)> points, double areaW, double areaH)
    {
        double cx = areaW / 2.0, cy = areaH / 2.0;
        int best = -1; double bestD = double.MaxValue;
        for (int i = 0; i < points.Count && i < provinces.Count; i++)
        {
            double dx = points[i].X - cx, dy = points[i].Y - cy;
            double d = dx * dx + dy * dy;
            if (d < bestD) { bestD = d; best = i; }
        }
        return best;
    }

    private static Terrain ClassifyTerrain(double h, double m, bool coast)
    {
        if (coast) return Terrain.Coast;
        if (h > 0.70) return Terrain.Mountains;
        if (h > 0.55) return Terrain.Hills;
        if (h > 0.35) return m > 0.60 ? Terrain.Forest : Terrain.Plains;
        return m > 0.70 ? Terrain.Marsh : Terrain.Plains;
    }

    private static Climate ClassifyClimate(double nx, double ny, double h, double m)
    {
        if (h > 0.65) return Climate.Cold;
        if (ny < 0.25 || ny > 0.75) return Climate.Cold;
        if (m < 0.35) return Climate.Arid;
        if (ny > 0.40 && ny < 0.60 && m > 0.55) return Climate.Mediterranean;
        return Climate.Temperate;
    }

    private static int TerrainBonus(Terrain t) => t switch
    {
        Terrain.Plains => 2,
        Terrain.Coast => 1,
        Terrain.Forest => 1,
        Terrain.Hills => 0,
        Terrain.Mountains => -2,
        Terrain.Marsh => -2,
        _ => 0
    };

    private static double FractalValueNoise(double nx, double ny, int octaves, ulong seed)
    {
        double amp = 1.0, freq = 1.0, total = 0.0, norm = 0.0;
        for (int o = 0; o < octaves; o++)
        {
            total += ValueNoise.Noise2D(nx * freq * 4.0, ny * freq * 4.0, seed + (ulong)o * 2654435761UL) * amp;
            norm += amp;
            amp *= 0.5;
            freq *= 2.0;
        }
        return norm > 0 ? total / norm : 0.5;
    }

    // Poisson-disc com ajuste de distância mínima para atingir a contagem alvo.
    private static List<(double X, double Y)> GeneratePoissonTargetCount(double w, double h, int target, double minDist, long seed)
    {
        if (target <= 0) return new List<(double X, double Y)>();
        double lo = minDist * 0.25, hi = minDist * 4.0;
        List<(double X, double Y)> best = null!;
        int bestDiff = int.MaxValue;
        int seed0 = (int)(seed & 0x7FFFFFFF);
        for (int it = 0; it < 22; it++)
        {
            double mid = (lo + hi) / 2.0;
            var pts = PoissonDisc.Generate(w, h, mid, seed0);
            int diff = Math.Abs(pts.Count - target);
            if (diff < bestDiff) { bestDiff = diff; best = pts; }
            if (bestDiff == 0) break;
            if (pts.Count < target) hi = mid;
            else lo = mid;
        }
        return best ?? PoissonDisc.Generate(w, h, minDist, seed0);
    }

    // Relaxamento de Lloyd: move cada site para o centróide de sua célula de Voronoi.
    private static List<(double X, double Y)> LloydRelax(List<(double X, double Y)> points, double areaW, double areaH, int width, int height, int iterations)
    {
        var pts = new List<(double X, double Y)>(points);
        if (pts.Count == 0) return pts;
        for (int it = 0; it < iterations; it++)
        {
            var nearest = AssignNearest(pts, width, height, areaW, areaH);
            var sumX = new double[pts.Count];
            var sumY = new double[pts.Count];
            var cnt = new int[pts.Count];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = nearest[y * width + x];
                    if (i < 0) continue;
                    double wx = x * areaW / width;
                    double wy = y * areaH / height;
                    sumX[i] += wx; sumY[i] += wy; cnt[i]++;
                }
            }
            bool moved = false;
            for (int i = 0; i < pts.Count; i++)
            {
                if (cnt[i] == 0) continue;
                double nx = sumX[i] / cnt[i], ny = sumY[i] / cnt[i];
                nx = Math.Clamp(nx, 0, areaW); ny = Math.Clamp(ny, 0, areaH);
                if (Math.Abs(nx - pts[i].X) > 1e-6 || Math.Abs(ny - pts[i].Y) > 1e-6) moved = true;
                pts[i] = (nx, ny);
            }
            if (!moved) break;
        }
        return pts;
    }

    // Atribui cada pixel ao site mais próximo usando grade espacial (O(pixels × células vizinhas)).
    private static int[] AssignNearest(List<(double X, double Y)> points, int width, int height, double areaW, double areaH)
    {
        int n = points.Count;
        var nearest = new int[width * height];
        if (n == 0) { Array.Fill(nearest, -1); return nearest; }

        double cellSize = Math.Max(1.0, Math.Sqrt(areaW * areaH / n));
        int cols = (int)Math.Ceiling(areaW / cellSize) + 2;
        var grid = new Dictionary<int, List<int>>();
        for (int i = 0; i < n; i++)
        {
            int cx = (int)(points[i].X / cellSize);
            int cy = (int)(points[i].Y / cellSize);
            int key = (cy + 1) * cols + (cx + 1);
            if (!grid.TryGetValue(key, out var list)) { list = new List<int>(); grid[key] = list; }
            list.Add(i);
        }

        for (int y = 0; y < height; y++)
        {
            double wy = y * areaH / height;
            for (int x = 0; x < width; x++)
            {
                double wx = x * areaW / width;
                int cx = (int)(wx / cellSize);
                int cy = (int)(wy / cellSize);
                int best = -1; double bestD = double.MaxValue;

                Scan(grid, points, cols, cellSize, wx, wy, cx, cy, 1, ref best, ref bestD);
                int R = (int)(Math.Sqrt(bestD) / cellSize) + 2;
                Scan(grid, points, cols, cellSize, wx, wy, cx, cy, R, ref best, ref bestD);

                nearest[y * width + x] = best;
            }
        }
        return nearest;
    }

    private static void Scan(Dictionary<int, List<int>> grid, List<(double X, double Y)> points, int cols,
        double cellSize, double wx, double wy, int cx, int cy, int R, ref int best, ref double bestD)
    {
        for (int dy = -R; dy <= R; dy++)
        {
            for (int dx = -R; dx <= R; dx++)
            {
                int key = (cy + dy + 1) * cols + (cx + dx + 1);
                if (!grid.TryGetValue(key, out var list)) continue;
                foreach (int i in list)
                {
                    double ddx = points[i].X - wx;
                    double ddy = points[i].Y - wy;
                    double d = ddx * ddx + ddy * ddy;
                    if (d < bestD) { bestD = d; best = i; }
                }
            }
        }
    }

    private static List<(MaritimeZone Zone, List<int> Cells)> BuildMaritimeZones(ushort[] raster, int width, int height, int firstZoneId)
    {
        var result = new List<(MaritimeZone, List<int>)>();
        bool[] visited = new bool[raster.Length];
        int nextId = firstZoneId;
        for (int start = 0; start < raster.Length; start++)
        {
            if (raster[start] != 0 || visited[start]) continue;
            var cells = new List<int>();
            var stack = new Stack<int>();
            stack.Push(start);
            visited[start] = true;
            while (stack.Count > 0)
            {
                int cur = stack.Pop();
                cells.Add(cur);
                int cy = cur / width, cx = cur % width;
                int[] nbX = { cx - 1, cx + 1, cx, cx };
                int[] nbY = { cy, cy, cy - 1, cy + 1 };
                for (int k = 0; k < 4; k++)
                {
                    if (nbX[k] < 0 || nbX[k] >= width || nbY[k] < 0 || nbY[k] >= height) continue;
                    int nidx = nbY[k] * width + nbX[k];
                    if (raster[nidx] == 0 && !visited[nidx]) { visited[nidx] = true; stack.Push(nidx); }
                }
            }
            result.Add((
                new MaritimeZone
                {
                    Id = new EntityId(nextId++),
                    Name = $"ZM{result.Count + 1}",
                    AdjacentProvinceIds = new List<EntityId>(),
                    HasPortAccess = true
                },
                cells));
        }
        return result;
    }
}
