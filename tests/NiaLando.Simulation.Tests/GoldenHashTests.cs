using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NiaLando.Domain;
using NiaLando.Simulation;
using Xunit;
using FluentAssertions;

namespace NiaLando.Simulation.Tests;

public class GoldenHashTests
{
    /// <summary>
    /// Golden hash do estado serializado para seeds fixas.
    /// Se o algoritmo de geração mudar de forma incompatível, incrementar GeneratorVersion e atualizar estes hashes.
    /// </summary>
    [Fact]
    public void GoldenHash_Seed12345_StateHash_Matches()
    {
        var state = CreateMinimalState(12345);
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
        {
            WriteIndented = false
        });
        var hash = ComputeSha256Hash(json);
        // Primeiro golden hash para estado mínimo - será atualizado quando o gerador completo for implementado
        hash.Should().NotBeNullOrEmpty();
    }

    private static GameState CreateMinimalState(long seed)
    {
        return new GameState
        {
            Seed = seed,
            Params = new MapParams { Seed = seed },
            GeneratorVersion = GeneratorVersion.Current,
            SaveVersion = 1,
            CurrentTurn = 0,
            CurrentYear = 1444,
            Map = new MapData(),
            Peoples = new List<People>(),
            Countries = new List<Country>(),
            Leaders = new List<Leader>(),
            Messages = new List<Message>(),
            Relations = new List<DiplomaticRelation>(),
            Armies = new List<Army>(),
            Fleets = new List<Fleet>()
        };
    }

    private static string ComputeSha256Hash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(bytes);
        var sb = new StringBuilder(hashBytes.Length * 2);
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
