using System;
using System.Collections.Generic;
using System.Linq;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class MapGenerator
{
    public GameState Generate(MapParams mapParams)
    {
        int width = Math.Max(64, 512);
        int height = Math.Max(64, 512);
        // Usar raster a partir de MapParams (padrão 512x512 se não especificado, mas respeitar intenção)
        if (mapParams.Tamanho > 0) { width = 512; height = 512; }
        
        var rng = new Xoshiro256StarStar((ulong)mapParams.Seed);
        var state = new GameState
        {
            Seed = mapParams.Seed,
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

        double minDist = mapParams.DistanciaMinima;
        if (minDist <= 0) minDist = 33.33;
        var points = PoissonDisc.Generate(mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000, mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000, minDist, (int)(mapParams.Seed & 0x7FFFFFFF));
        
        // Ajustar distância mínima para tentar aproximar numProvinciasAlvo (sem cortar pontos)
        for (int iter = 0; iter < 10 && mapParams.NumProvinciasAlvo > 0 && points.Count < mapParams.NumProvinciasAlvo; iter++)
        {
            minDist *= 0.9;
            points = PoissonDisc.Generate(mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000, mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000, minDist, (int)((mapParams.Seed + iter * 131542391) & 0x7FFFFFFF));
        }
        for (int iter = 0; iter < 10 && mapParams.NumProvinciasAlvo > 0 && points.Count > mapParams.NumProvinciasAlvo * 1.5; iter++)
        {
            minDist *= 1.1;
            points = PoissonDisc.Generate(mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000, mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000, minDist, (int)((mapParams.Seed + iter * 1009) & 0x7FFFFFFF));
        }
        
        // Aplicar Lloyd relaxation
        int lloydIter = mapParams.IteracoesLloyd;
        if (lloydIter < 0) lloydIter = 0;
        for (int li = 0; li < lloydIter; li++)
        {
            var cells = new List<List<int>>();
            for (int i = 0; i < points.Count; i++) cells.Add(new List<int>());
            double scaleX = width / (mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000);
            double scaleY = height / (mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int best = 0;
                    double bestD = double.MaxValue;
                    for (int p = 0; p < points.Count; p++)
                    {
                        double dx = points[p].X * scaleX - x;
                        double dy = points[p].Y * scaleY - y;
                        double d = dx * dx + dy * dy;
                        if (d < bestD) { bestD = d; best = p; }
                    }
                    cells[best].Add(y * width + x);
                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                if (cells[i].Count == 0) continue;
                double sx = 0, sy = 0;
                foreach (var idx in cells[i])
                {
                    sx += idx % width;
                    sy += idx / width;
                }
                points[i] = (sx / cells[i].Count / scaleX, sy / cells[i].Count / scaleY);
            }
        }
        
        // Gerar altura com ValueNoise fractal
        var heightMap = new double[width * height];
        int octaves = mapParams.OctavasRuido > 0 ? mapParams.OctavasRuido : 5;
        ulong noiseSeed = (ulong)mapParams.Seed ^ 0x123456789ABCDEFUL;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double nx = x / (double)width;
                double ny = y / (double)height;
                double amp = 1.0, freq = 1.0, total = 0.0, norm = 0.0;
                for (int o = 0; o < octaves; o++)
                {
                    total += ValueNoise.Noise2D(nx * freq * 8.0, ny * freq * 8.0, noiseSeed + (ulong)o * 131542391UL) * amp;
                    norm += amp;
                    amp *= 0.5;
                    freq *= 2.0;
                }
                heightMap[y * width + x] = norm > 0 ? total / norm : 0.5;
            }
        }
        
        // Limiar por quantil para atingir PorcentagemTerra
        double targetLand = mapParams.PorcentagemTerra > 0 && mapParams.PorcentagemTerra < 1 ? mapParams.PorcentagemTerra : 0.45;
        var sorted = heightMap.OrderBy(v => v).ToArray();
        int landCountTarget = (int)(width * height * targetLand);
        double threshold = sorted[Math.Clamp(landCountTarget, 0, sorted.Length - 1)];
        
        double scaleX2 = width / (mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000);
        double scaleY2 = height / (mapParams.Tamanho > 0 ? mapParams.Tamanho : 1000);
        var provinceList = new List<Province>();
        int nextId = 1;
        int provinceCount = points.Count;
        for (int i = 0; i < provinceCount; i++)
        {
            var province = new Province
            {
                Id = new EntityId(nextId++),
                Name = $"Prov{i + 1}",
                Terrain = Terrain.Plains,
                Climate = Climate.Temperate,
                Development = 5,
                TradeValue = 1,
                HasPort = false,
                FortLevel = 0,
                AdjacentProvinceIds = new List<EntityId>(),
                Populations = new List<ProvincePopulation>()
            };
            provinceList.Add(province);
        }
        
        // Raster: 0 = mar, >0 = província
        var provIdBySite = new int[provinceCount];
        for (int i = 0; i < provinceCount; i++) provIdBySite[i] = provinceList[i].Id.Value;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                if (heightMap[idx] < threshold)
                {
                    state.Map.ProvinceIdRaster[idx] = 0;
                    continue;
                }
                int best = 0;
                double bestD = double.MaxValue;
                for (int p = 0; p < points.Count; p++)
                {
                    double dx = points[p].X * scaleX2 - x;
                    double dy = points[p].Y * scaleY2 - y;
                    double d = dx * dx + dy * dy;
                    if (d < bestD) { bestD = d; best = p; }
                }
                state.Map.ProvinceIdRaster[idx] = (ushort)provIdBySite[best];
            }
        }
        
        // Adjacências derivadas do raster (4-neigh)
        var adjSet = new Dictionary<int, HashSet<int>>();
        for (int i = 0; i < provinceList.Count; i++) adjSet[provinceList[i].Id.Value] = new HashSet<int>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                ushort cur = state.Map.ProvinceIdRaster[idx];
                if (cur == 0) continue;
                if (x + 1 < width)
                {
                    ushort n = state.Map.ProvinceIdRaster[idx + 1];
                    if (n != 0 && n != cur)
                    {
                        adjSet[cur].Add(n);
                        adjSet[n].Add(cur);
                    }
                }
                if (y + 1 < height)
                {
                    ushort n2 = state.Map.ProvinceIdRaster[idx + width];
                    if (n2 != 0 && n2 != cur)
                    {
                        adjSet[cur].Add(n2);
                        adjSet[n2].Add(cur);
                    }
                }
            }
        }
        foreach (var prov in provinceList)
        {
            prov.AdjacentProvinceIds = adjSet[prov.Id.Value].Select(v => new EntityId(v)).ToList();
            // Porto se adjacente a água
            bool hasWaterNeighbor = false;
            // verificacao simplificada via raster vizinhos
            prov.HasPort = prov.AdjacentProvinceIds.Count > 0 && hasWaterNeighbor == false; // será refinado
        }
        // Verificar portos por raster
        foreach (var prov in provinceList)
        {
            bool hasWater = false;
            for (int y = 0; y < height && !hasWater; y++)
            {
                for (int x = 0; x < width && !hasWater; x++)
                {
                    if (state.Map.ProvinceIdRaster[y * width + x] == prov.Id.Value)
                    {
                        if ((x > 0 && state.Map.ProvinceIdRaster[y * width + x - 1] == 0) ||
                            (x < width - 1 && state.Map.ProvinceIdRaster[y * width + x + 1] == 0) ||
                            (y > 0 && state.Map.ProvinceIdRaster[(y - 1) * width + x] == 0) ||
                            (y < height - 1 && state.Map.ProvinceIdRaster[(y + 1) * width + x] == 0))
                            hasWater = true;
                    }
                }
            }
            prov.HasPort = hasWater;
        }
        
        state.Map.Provinces = provinceList;
        return state;
    }
}
