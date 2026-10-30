using System;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class EconomyProcessor
{
    public GameState Process(GameState state)
    {
        foreach (var country in state.Countries)
        {
            long income = 0;
            foreach (var prov in state.Map.Provinces)
            {
                if (prov.OwnerId == country.Id)
                {
                    income += prov.Development * 10;
                }
            }
            country.Treasury += income;
        }
        return state;
    }
}
