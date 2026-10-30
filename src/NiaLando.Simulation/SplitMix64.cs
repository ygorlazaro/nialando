namespace NiaLando.Simulation;

/// <summary>
/// SplitMix64: usada para derivar sub-streams do PRNG a partir de (gameSeed, turn, systemId).
/// e para inicializar o estado do xoshiro256**.
/// </summary>
public static class SplitMix64
{
    private const ulong Mult = 0x9E3779B97F4A7C15UL;

    public static ulong Next(ref ulong state)
    {
        state += Mult;
        ulong z = state;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }

    /// <summary>
    /// Deriva um Xoshiro256** a partir de uma seed de 64 bits.
    /// </summary>
    public static (ulong, ulong, ulong, ulong) Seed(ulong seed)
    {
        ulong state = seed;
        return (Next(ref state), Next(ref state), Next(ref state), Next(ref state));
    }

    /// <summary>
    /// Deriva uma sub-stream a partir de (gameSeed, turn, systemId).
    /// Mexer em um sistema não altera o RNG dos outros.
    /// </summary>
    public static Xoshiro256StarStar SubStream(long gameSeed, int turn, int systemId)
    {
        // Combina os três em um estado inicial do SplitMix64.
        ulong state = (ulong)gameSeed;
        state ^= (ulong)turn * 0x9E3779B97F4A7C15UL;
        state ^= (ulong)systemId * 0xD1B54A32D192ED03UL;
        state ^= 0x2545F4914F6CDD1DUL;
        return new Xoshiro256StarStar(state);
    }
}