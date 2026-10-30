using System.Collections.Generic;

namespace NiaLando.Simulation;

public sealed class NameGenerator
{
    private static readonly string[] Prefixes = { "Ald", "Ber", "Cor", "Dor", "El", "Fen", "Gal", "Hor", "Ith", "Kal", "Lor", "Mer", "Nor", "Ost", "Pel", "Quel", "Rav", "Sol", "Tel", "Val" };
    private static readonly string[] Suffixes = { "en", "ia", "is", "on", "um", "ar", "eth", "in", "or", "es" };
    private static readonly string[] CountryPrefixes = { "Regno", "Ducato", "Contea", "Marka", "Principato" };
    
    public string GenerateProvinceName(Xoshiro256StarStar rng, string style)
    {
        string prefix = Prefixes[rng.NextInt(Prefixes.Length)];
        string suffix = Suffixes[rng.NextInt(Suffixes.Length)];
        return prefix + suffix;
    }
    
    public string GenerateCountryName(Xoshiro256StarStar rng)
    {
        string prefix = CountryPrefixes[rng.NextInt(CountryPrefixes.Length)];
        string name = Prefixes[rng.NextInt(Prefixes.Length)] + Suffixes[rng.NextInt(Suffixes.Length)];
        return prefix + " de " + name;
    }
    
    public string GenerateLeaderName(Xoshiro256StarStar rng)
    {
        return Prefixes[rng.NextInt(Prefixes.Length)] + "o " + Suffixes[rng.NextInt(Suffixes.Length)];
    }
}
