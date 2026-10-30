using NiaLando.Domain;
using NiaLando.Simulation;
using Xunit;
using FluentAssertions;

namespace NiaLando.Simulation.Tests;

public class SetupTests
{
    [Fact]
    public void Setup_CreatesCountriesAndLeaders()
    {
        var gen = new MapGenerator();
        var setup = new SetupGenerator();
        var state = gen.Generate(new MapParams { Seed = 1234, NumProvinciasAlvo = 100 });
        state = setup.Setup(state, 10);
        
        state.Countries.Should().NotBeEmpty();
        state.Leaders.Should().NotBeEmpty();
        state.Peoples.Should().NotBeEmpty();
        state.PlayerCountryId.Should().NotBeNull();
    }

    [Fact]
    public void LeaderAttributes_ValidRange()
    {
        var attrs = new LeaderAttributes(50, 75, 25);
        attrs.Diplomacy.Should().Be(50);
        attrs.Violence.Should().Be(75);
        attrs.Greed.Should().Be(25);
        attrs.IsValid().Should().BeTrue();
        
        var attrs2 = new LeaderAttributes(5, 150, 50);
        attrs2.Diplomacy.Should().Be(10);
        attrs2.Violence.Should().Be(100);
    }
}
