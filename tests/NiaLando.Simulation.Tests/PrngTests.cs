using NiaLando.Simulation;
using Xunit;
using FluentAssertions;

namespace NiaLando.Simulation.Tests;

public class PrngTests
{
    /// <summary>
    /// Vetores de referência gerados por uma implementação de referência em Python
    /// (xoshiro256** com SplitMix64 de seed). Se estes mudarem, o algoritmo mudou.
    /// </summary>
    [Fact]
    public void Xoshiro256StarStar_FirstEightUInt64_MatchReference()
    {
        var rng = new Xoshiro256StarStar(12345);
        var actual = new ulong[8];
        for (int i = 0; i < 8; i++) actual[i] = rng.NextUInt64();

        actual[0].Should().Be(13720838825685603483UL);
        actual[1].Should().Be(2398916695208396998UL);
        actual[2].Should().Be(17770384849984869256UL);
        actual[3].Should().Be(891717726879801395UL);
        actual[4].Should().Be(10241316046318454344UL);
        actual[5].Should().Be(196975429884907396UL);
        actual[6].Should().Be(2947371003896198809UL);
        actual[7].Should().Be(5456629693515947710UL);
    }

    [Fact]
    public void NextDouble_FirstEight_MatchReference()
    {
        var rng = new Xoshiro256StarStar(12345);
        rng.NextDouble().Should().Be(0.7438081631565894);
        rng.NextDouble().Should().Be(0.13004553462783452);
        rng.NextDouble().Should().Be(0.9633344930128545);
        rng.NextDouble().Should().Be(0.048340114836345816);
        rng.NextDouble().Should().Be(0.5551828553264562);
        rng.NextDouble().Should().Be(0.010678059450374033);
        rng.NextDouble().Should().Be(0.15977730227725195);
        rng.NextDouble().Should().Be(0.29580448840794504);
    }

    [Fact]
    public void NextInt_FirstEight_MatchReference()
    {
        var rng = new Xoshiro256StarStar(12345);
        rng.NextInt(100).Should().Be(74);
        rng.NextInt(100).Should().Be(13);
        rng.NextInt(100).Should().Be(96);
        rng.NextInt(100).Should().Be(4);
        rng.NextInt(100).Should().Be(55);
        rng.NextInt(100).Should().Be(1);
        rng.NextInt(100).Should().Be(15);
        rng.NextInt(100).Should().Be(29);
    }

    [Fact]
    public void NextBool_FirstEight_MatchReference()
    {
        var rng = new Xoshiro256StarStar(12345);
        rng.NextBool().Should().Be(false);
        rng.NextBool().Should().Be(true);
        rng.NextBool().Should().Be(false);
        rng.NextBool().Should().Be(true);
        rng.NextBool().Should().Be(false);
        rng.NextBool().Should().Be(true);
        rng.NextBool().Should().Be(true);
        rng.NextBool().Should().Be(true);
    }

    [Fact]
    public void SubStream_IndependentBetweenSystems()
    {
        var a = SplitMix64.SubStream(999, 5, 0);
        var b = SplitMix64.SubStream(999, 5, 1);
        var c = SplitMix64.SubStream(999, 6, 0);

        var va = new ulong[4];
        var vb = new ulong[4];
        var vc = new ulong[4];
        for (int i = 0; i < 4; i++)
        {
            va[i] = a.NextUInt64();
            vb[i] = b.NextUInt64();
            vc[i] = c.NextUInt64();
        }

        // Reference vectors
        va[0].Should().Be(13535675325450855262UL);
        vb[0].Should().Be(13944133535157841543UL);
        vc[0].Should().Be(9369173802902463717UL);

        // Different systems must diverge
        va.Should().NotEqual(vb);
        va.Should().NotEqual(vc);
        vb.Should().NotEqual(vc);
    }

    [Fact]
    public void SameSeed_ProducesSameSequence()
    {
        var a = new Xoshiro256StarStar(42);
        var b = new Xoshiro256StarStar(42);
        for (int i = 0; i < 100; i++)
            a.NextUInt64().Should().Be(b.NextUInt64());
    }

    [Fact]
    public void DifferentSeed_ProducesDifferentSequence()
    {
        var a = new Xoshiro256StarStar(42);
        var b = new Xoshiro256StarStar(43);
        var equal = 0;
        for (int i = 0; i < 100; i++)
            if (a.NextUInt64() == b.NextUInt64()) equal++;
        equal.Should().BeLessThan(5);
    }
}