using NiaLando.Application;
using NiaLando.Domain;
using NiaLando.Simulation;
using System.Text.Json;

namespace NiaLando.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
        long seed = 12345;
        int turns = 10;
        string? outPath = null;
        
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--seed" && i + 1 < args.Length)
                seed = long.Parse(args[i + 1]);
            else if (args[i] == "--turns" && i + 1 < args.Length)
                turns = int.Parse(args[i + 1]);
            else if (args[i] == "--out" && i + 1 < args.Length)
                outPath = args[i + 1];
        }
        
        var generator = new MapGenerator();
        var setup = new SetupGenerator();
        var processor = new TurnProcessor();
        
        var state = generator.Generate(new MapParams { Seed = seed, NumProvinciasAlvo = 200 });
        state = setup.Setup(state, 8);
        
        for (int t = 0; t < turns; t++)
        {
            state = processor.EndTurn(state);
        }
        
        var report = new
        {
            seed,
            turns,
            finalTurn = state.CurrentTurn,
            provinces = state.Map.Provinces.Count,
            countries = state.Countries.Count,
            messages = state.Messages.Count,
            hash = ComputeHash(state)
        };
        
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(json);
        
        if (outPath != null)
        {
            File.WriteAllText(outPath, json);
        }
        
        return 0;
    }
    
    private static string ComputeHash(GameState state)
    {
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = false });
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var hashBytes = System.Security.Cryptography.SHA256.HashData(bytes);
        var sb = new System.Text.StringBuilder(hashBytes.Length * 2);
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
