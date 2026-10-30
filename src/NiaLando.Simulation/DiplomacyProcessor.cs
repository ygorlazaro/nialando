using System;
using System.Collections.Generic;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class DiplomacyProcessor
{
    private int _proposalId = 1;
    private readonly Xoshiro256StarStar _rng = new(12345);
    
    public GameState Process(GameState state)
    {
        // IA utility-based simplificado
        if (state.Countries.Count >= 2 && state.PendingProposals.Count < 3)
        {
            var from = state.Countries[_rng.NextInt(state.Countries.Count)];
            var to = state.Countries[_rng.NextInt(state.Countries.Count)];
            if (from.Id != to.Id && (state.PlayerCountryId == null || to.Id == state.PlayerCountryId || from.Id == state.PlayerCountryId))
            {
                state.PendingProposals.Add(new PendingProposal
                {
                    Id = _proposalId++,
                    FromCountryId = from.Id,
                    Type = "alliance",
                    Key = "prop.alliance.offer",
                    Parameters = new Dictionary<string, string> { { "from", from.Name } },
                    IsSealed = true
                });
            }
        }
        return state;
    }
}
