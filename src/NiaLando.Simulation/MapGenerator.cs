using System;
using System.Collections.Generic;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class MapGenerator
{
    public GameState Generate(MapParams mapParams)
    {
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
                Width = 512,
                Height = 512,
                GeneratorVersion = GeneratorVersion.Current,
                ProvinceIdRaster = new ushort[512 * 512]
            }
        };

        var points = PoissonDisc.Generate(mapParams.Tamanho, mapParams.Tamanho, mapParams.DistanciaMinima, (int)(mapParams.Seed & 0x7FFFFFFF));
        var provinceCount = Math.Min(points.Count, mapParams.NumProvinciasAlvo);
        
        var nextId = 1;
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
            state.Map.Provinces.Add(province);
        }

        // Gerar raster simples (mapeia pixels para províncias por proximidade)
        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                int bestIdx = 0;
                double bestDist = double.MaxValue;
                for (int p = 0; p < provinceCount && p < points.Count; p++)
                {
                    double dx = points[p].X * 512.0 / mapParams.Tamanho - x;
                    double dy = points[p].Y * 512.0 / mapParams.Tamanho - y;
                    double dist = dx * dx + dy * dy;
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestIdx = p;
                    }
                }
                if (provinceCount > 0)
                    state.Map.ProvinceIdRaster[y * 512 + x] = (ushort)state.Map.Provinces[bestIdx].Id.Value;
                else
                    state.Map.ProvinceIdRaster[y * 512 + x] = 0;
            }
        }

        // Garantir adjacências simétricas básicas
        for (int i = 0; i < state.Map.Provinces.Count; i++)
        {
            for (int j = i + 1; j < state.Map.Provinces.Count; j++)
            {
                if (rng.NextDouble() < 0.1)
                {
                    state.Map.Provinces[i].AdjacentProvinceIds.Add(state.Map.Provinces[j].Id);
                    state.Map.Provinces[j].AdjacentProvinceIds.Add(state.Map.Provinces[i].Id);
                }
            }
        }

        return state;
    }
}
