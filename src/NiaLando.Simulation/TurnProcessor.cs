using System;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class TurnProcessor
{
    private readonly EconomyProcessor _economy = new();
    private readonly RecruitmentProcessor _recruitment = new();
    private readonly NavalProcessor _naval = new();
    private readonly CombatProcessor _combat = new();
    private readonly DiplomacyProcessor _diplomacy = new();
    private readonly PeopleProcessor _peopleProc = new();
    
    public GameState EndTurn(GameState state)
    {
        state = _economy.Process(state);
        state = _recruitment.Process(state);
        state = _naval.Process(state);
        state = _diplomacy.Process(state);
        state = ProcessMovement(state);
        state = _combat.Process(state);
        state = ProcessPeace(state);
        state = _peopleProc.Process(state);
        state = ProcessEvents(state);
        state = ProcessSuccession(state);
        
        state.CurrentTurn++;
        state.CurrentYear = 1444 + (state.CurrentTurn / 12);
        
        return state;
    }
    
    private GameState ProcessMovement(GameState state) => state;
    private GameState ProcessPeace(GameState state) => state;
    private GameState ProcessEvents(GameState state) => state;
    private GameState ProcessSuccession(GameState state) => state;
}
