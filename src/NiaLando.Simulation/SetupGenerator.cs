using System;
using System.Collections.Generic;
using System.Linq;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class SetupGenerator
{
    public GameState Setup(GameState state, int numCountries = 12)
    {
        var rng = new Xoshiro256StarStar((ulong)(state.Seed ^ 0x5EED));
        var nameGen = new NameGenerator();
        var nextId = state.Map.Provinces.Count + 1;
        
        // Criar povos
        var peopleCount = Math.Min(8, numCountries);
        for (int i = 0; i < peopleCount; i++)
        {
            state.Peoples.Add(new People
            {
                Id = new EntityId(nextId++),
                Name = nameGen.GenerateProvinceName(rng, "default"),
                CultureStyle = "default"
            });
        }
        
        // Criar países
        var provincesPerCountry = Math.Max(1, state.Map.Provinces.Count / numCountries);
        var provIndex = 0;
        for (int c = 0; c < numCountries && provIndex < state.Map.Provinces.Count; c++)
        {
            var leader = new Leader
            {
                Id = new EntityId(nextId++),
                Name = nameGen.GenerateLeaderName(rng),
                Age = 30 + rng.NextInt(20),
                Attributes = new LeaderAttributes(
                    10 + rng.NextInt(91),
                    10 + rng.NextInt(91),
                    10 + rng.NextInt(91)
                )
            };
            state.Leaders.Add(leader);
            
            var country = new Country
            {
                Id = new EntityId(nextId++),
                Name = nameGen.GenerateCountryName(rng),
                CapitalProvinceId = state.Map.Provinces[provIndex].Id,
                RulerId = leader.Id,
                DominantPeopleId = state.Peoples[c % state.Peoples.Count].Id,
                Treasury = 1000
            };
            state.Countries.Add(country);
            
            // Atribuir províncias
            int count = provincesPerCountry;
            if (c == numCountries - 1) count = state.Map.Provinces.Count - provIndex;
            for (int p = 0; p < count && provIndex < state.Map.Provinces.Count; p++)
            {
                state.Map.Provinces[provIndex].OwnerId = country.Id;
                state.Map.Provinces[provIndex].ControllerId = country.Id;
                // Distribuir população
                if (state.Peoples.Count > 0)
                {
                    state.Map.Provinces[provIndex].Populations.Add(new ProvincePopulation
                    {
                        PeopleId = state.Peoples[c % state.Peoples.Count].Id,
                        Count = 10000 + rng.NextInt(50000)
                    });
                }
                provIndex++;
            }
        }
        
        // Sortear país do jogador
        if (state.Countries.Count > 0)
        {
            var playerIdx = rng.NextInt(state.Countries.Count);
            state.PlayerCountryId = state.Countries[playerIdx].Id;
        }
        
        return state;
    }
}
