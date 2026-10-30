using System;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class CombatProcessor
{
    public GameState Process(GameState state)
    {
        // Combate terrestre básico
        if (state.Armies.Count >= 2)
        {
            var attacker = state.Armies[0];
            var defender = state.Armies[1];
            // Resultado determinístico simplificado
            if (attacker.Size > defender.Size)
                defender.Size = (int)(defender.Size * 0.7);
            else
                attacker.Size = (int)(attacker.Size * 0.7);
        }
        return state;
    }
}
