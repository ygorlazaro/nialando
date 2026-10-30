using System;
using System.Collections.Generic;
using System.Linq;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class PeopleProcessor
{
    private int _nextId = 10000;
    private readonly Xoshiro256StarStar _rng = new(777);
    
    public GameState Process(GameState state)
    {
        // Irredentismo e separatismo simplificados
        if (state.Countries.Count >= 2 && _rng.NextDouble() < 0.1)
        {
            // Criar país novo por revolta separatista
            var people = state.Peoples[_rng.NextInt(state.Peoples.Count)];
            var leader = new Leader
            {
                Id = new EntityId(_nextId++),
                Name = people.Name + " Leader",
                Age = 25,
                Attributes = new LeaderAttributes(30 + _rng.NextInt(40), 40 + _rng.NextInt(40), 20 + _rng.NextInt(30))
            };
            state.Leaders.Add(leader);
            
            var newCountry = new Country
            {
                Id = new EntityId(_nextId++),
                Name = "Nação de " + people.Name,
                RulerId = leader.Id,
                DominantPeopleId = people.Id,
                Treasury = 500
            };
            state.Countries.Add(newCountry);
            
            state.Messages.Add(new Message
            {
                Id = state.Messages.Count + 1,
                Turn = state.CurrentTurn + 1,
                Category = "peoples",
                Importance = 3,
                Key = "msg.peoples.revolt",
                Parameters = new Dictionary<string, string> { { "people", people.Name }, { "country", newCountry.Name } },
                InvolvedEntities = new List<EntityId> { newCountry.Id },
                IsRead = false,
                RequiresDecision = false
            });
        }
        return state;
    }
}
