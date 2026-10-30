using System;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class RecruitmentProcessor
{
    public GameState Process(GameState state)
    {
        // Recrutamento básico
        foreach (var country in state.Countries)
        {
            if (country.Treasury >= 1000 && state.Armies.Count < country.Id.Value * 10)
            {
                state.Armies.Add(new Army
                {
                    Id = new EntityId(state.Armies.Count + 1000),
                    CountryId = country.Id,
                    ProvinceId = country.CapitalProvinceId,
                    Size = 10000
                });
                country.Treasury -= 1000;
            }
        }
        return state;
    }
}
