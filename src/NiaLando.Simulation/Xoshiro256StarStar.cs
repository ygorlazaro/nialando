namespace NiaLando.Simulation;

/// <summary>
/// Núcleo determinístico: xoshiro256** (Vigna/Blackman & See).
/// Usado em todo o motor de simulação.
///
/// Não usar System.Random, Guid.NewGuid(), DateTime.Now, GetHashCode() em Simulation.
/// </summary>
public sealed class Xoshiro256StarStar
{
    private ulong _s0, _s1, _s2, _s3;

    public Xoshiro256StarStar(ulong seed)
    {
        var split = SplitMix64.Seed(seed);
        _s0 = split.Item1;
        _s1 = split.Item2;
        _s2 = split.Item3;
        _s3 = split.Item4;
    }

    public ulong NextUInt64()
    {
        ulong result = Rotl(_s1 * 5, 7) * 9;
        ulong t = _s1 << 17;
        _s2 ^= _s0;
        _s3 ^= _s1;
        _s1 ^= _s2;
        _s0 ^= _s3;
        _s2 ^= t;
        _s3 = Rotl(_s3, 45);
        return result;
    }

    public uint NextUInt32() => (uint)NextUInt64();

    public double NextDouble()
    {
        // 53 bits de precisão
        return (NextUInt64() >> 11) * (1.0 / 9007199254740992.0);
    }

    public int NextInt(int max)
    {
        if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max));
        return (int)(NextDouble() * max);
    }

    public bool NextBool() => NextDouble() < 0.5;

    private static ulong Rotl(ulong x, int k) => (x << k) | (x >> (64 - k));
}
