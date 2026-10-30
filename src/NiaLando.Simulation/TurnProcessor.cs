using System;
using System.Collections.Generic;
using System.Linq;
using NiaLando.Domain;

namespace NiaLando.Simulation;

public sealed class TurnProcessor
{
    private int _messageIdCounter = 1;
    private readonly EconomyProcessor _economy = new();
    private readonly RecruitmentProcessor _recruitment = new();
    private readonly CombatProcessor _combat = new();
    private readonly DiplomacyProcessor _diplomacy = new();
    
    public GameState EndTurn(GameState state)
    {
        state = _economy.Process(state);
        state = _recruitment.Process(state);
        state = _diplomacy.Process(state);
        state = ProcessMovement(state);
        state = _combat.Process(state);
        state = ProcessPeace(state);
        state = ProcessPeople(state);
        state = ProcessEvents(state);
        state = ProcessSuccession(state);
        
        state.CurrentTurn++;
        state.CurrentYear = 1444 + (state.CurrentTurn / 12);
        
        return state;
    }
    
    private GameState ProcessMovement(GameState state) => state;
    private GameState ProcessPeace(GameState state) => state;
    
    private GameState ProcessPeople(GameState state)
    {
        if (state.Countries.Count > 0)
        {
            state.Messages.Add(new Message
            {
                Id = _messageIdCounter++,
                Turn = state.CurrentTurn + 1,
                Category = "diplomacy",
                Importance = 2,
                Key = "msg.turn.diplomacy",
                Parameters = new Dictionary<string, string>(),
                InvolvedEntities = new List<EntityId>(),
                IsRead = false,
                RequiresDecision = false
            });
        }
        return state;
    }
    
    private GameState ProcessEvents(GameState state) => state;
    private GameState ProcessSuccession(GameState state) => state;
}
