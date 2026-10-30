using System;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class NavalProcessor
{
    public GameState Process(GameState state)
    {
        // Criar frotas básicas
        foreach (var country in state.Countries)
        {
            if (country.Treasury >= 500 && state.Fleets.Count < country.Id.Value * 5)
            {
                var portProv = state.Map.Provinces.Find(p => p.OwnerId == country.Id && p.HasPort);
                if (portProv != null)
                {
                    state.Fleets.Add(new Fleet
                    {
                        Id = new EntityId(state.Fleets.Count + 2000),
                        CountryId = country.Id,
                        MaritimeZoneId = null,
                        Size = 10
                    });
                    country.Treasury -= 500;
                }
            }
        }
        return state;
    }
}
