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
}
