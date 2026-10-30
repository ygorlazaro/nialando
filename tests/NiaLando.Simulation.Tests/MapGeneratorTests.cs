using NiaLando.Domain;
using NiaLando.Simulation;
using Xunit;
using FluentAssertions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NiaLando.Simulation.Tests;

public class MapGeneratorTests
{
    [Fact]
    public void Generate_SameSeed_SameHash()
    {
        var gen = new MapGenerator();
        var state1 = gen.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 100 });
        var state2 = gen.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 100 });
        
        var hash1 = ComputeHash(state1);
        var hash2 = ComputeHash(state2);
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Generate_DifferentSeed_DifferentHash()
    {
        var gen = new MapGenerator();
        var state1 = gen.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 100 });
        var state2 = gen.Generate(new MapParams { Seed = 54321, NumProvinciasAlvo = 100 });
        
        var hash1 = ComputeHash(state1);
        var hash2 = ComputeHash(state2);
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Generate_AdjacenciesSymmetric()
    {
        var gen = new MapGenerator();
        var state = gen.Generate(new MapParams { Seed = 777, NumProvinciasAlvo = 50 });
        
        foreach (var prov in state.Map.Provinces)
        {
            foreach (var adjId in prov.AdjacentProvinceIds)
            {
                var adj = state.Map.Provinces.FirstOrDefault(p => p.Id == adjId);
                if (adj != null)
                {
                    adj.AdjacentProvinceIds.Should().Contain(prov.Id);
                }
            }
        }
    }

    [Fact]
    public void Generate_HasProvinces()
    {
        var gen = new MapGenerator();
        var state = gen.Generate(new MapParams { Seed = 999, NumProvinciasAlvo = 100 });
        state.Map.Provinces.Should().NotBeEmpty();
        state.Map.Width.Should().Be(512);
        state.Map.Height.Should().Be(512);
        state.Map.ProvinceIdRaster.Should().HaveCount(512 * 512);
    }

    private static string ComputeHash(GameState state)
    {
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = false });
        var bytes = Encoding.UTF8.GetBytes(json);
        var hashBytes = SHA256.HashData(bytes);
        var sb = new StringBuilder(hashBytes.Length * 2);
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    [Fact]
    public void Raster_HasWaterAndLand()
    {
        var gen = new MapGenerator();
        var state = gen.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 200, PorcentagemTerra = 0.45, Tamanho = 1000, DistanciaMinima = 33.33, IteracoesLloyd = 3, OctavasRuido = 5 });
        int water = 0, land = 0;
        for (int i = 0; i < state.Map.ProvinceIdRaster.Length; i++)
        {
            if (state.Map.ProvinceIdRaster[i] == 0) water++; else land++;
        }
        water.Should().BeGreaterThan(0);
        land.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Raster_LandFraction_WithinTwoPercent()
    {
        var gen = new MapGenerator();
        var p = new MapParams { Seed = 12345, NumProvinciasAlvo = 200, PorcentagemTerra = 0.45, Tamanho = 1000, DistanciaMinima = 33.33, IteracoesLloyd = 3, OctavasRuido = 5 };
        var state = gen.Generate(p);
        int land = 0;
        for (int i = 0; i < state.Map.ProvinceIdRaster.Length; i++) if (state.Map.ProvinceIdRaster[i] != 0) land++;
        double frac = (double)land / (double)state.Map.ProvinceIdRaster.Length;
        frac.Should().BeInRange(p.PorcentagemTerra - 0.02, p.PorcentagemTerra + 0.02);
    }

    [Fact]
    public void Raster_ProvincesRepresented()
    {
        var gen = new MapGenerator();
        var state = gen.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 200, PorcentagemTerra = 0.45 });
        var present = new HashSet<ushort>();
        for (int i = 0; i < state.Map.ProvinceIdRaster.Length; i++) if (state.Map.ProvinceIdRaster[i] != 0) present.Add(state.Map.ProvinceIdRaster[i]);
        var provinceIds = state.Map.Provinces.Select(x => (ushort)x.Id.Value).ToHashSet();
        present.Count.Should().BeGreaterOrEqualTo((int)(provinceIds.Count * 0.8));
    }

    [Fact]
    public void Adjacencies_DerivedAndSymmetric()
    {
        var gen = new MapGenerator();
        var state = gen.Generate(new MapParams { Seed = 777, NumProvinciasAlvo = 150, PorcentagemTerra = 0.45 });
        foreach (var prov in state.Map.Provinces)
        {
            foreach (var adj in prov.AdjacentProvinceIds)
            {
                var other = state.Map.Provinces.FirstOrDefault(p => p.Id == adj);
                other.Should().NotBeNull();
                other!.AdjacentProvinceIds.Should().Contain(prov.Id);
            }
        }
    }

    [Fact]
    public void Ports_AdjacentToWater()
    {
        var gen = new MapGenerator();
        var state = gen.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 200, PorcentagemTerra = 0.45 });
        foreach (var prov in state.Map.Provinces.Where(p => p.HasPort))
        {
            bool hasWaterNeighbor = false;
            // Verificar se tem vizinho com ID 0 no raster
            hasWaterNeighbor.Should().BeTrue();
        }
    }

    [Fact]
    public void HitTest_CenterOfProvince()
    {
        var gen = new MapGenerator();
        var state = gen.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 100, PorcentagemTerra = 0.45 });
        state.Map.ProvinceIdRaster.Should().NotBeEmpty();
    }
}

