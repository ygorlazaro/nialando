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
    
    public GameState EndTurn(GameState state)
    {
        // Fases fixas e determinísticas
        state = _economy.Process(state);
        state = _recruitment.Process(state);
        state = ProcessDiplomacy(state);
        state = ProcessMovement(state);
        state = ProcessBattles(state);
        state = ProcessPeace(state);
        state = ProcessPeople(state);
        state = ProcessEvents(state);
        state = ProcessSuccession(state);
        
        state.CurrentTurn++;
        state.CurrentYear = 1444 + (state.CurrentTurn / 12);
        
        return state;
    }
    
    private GameState ProcessDiplomacy(GameState state) => state;
    private GameState ProcessMovement(GameState state) => state;
    private GameState ProcessBattles(GameState state) => state;
    private GameState ProcessPeace(GameState state) => state;
    
    private GameState ProcessPeople(GameState state)
    {
        if (state.Countries.Count > 0)
        {
            state.Messages.Add(new Message
            {
                Id = _messageIdCounter++,
                Turn = state.CurrentTurn + 1,
                Category = "economy",
                Importance = 1,
                Key = "msg.turn.economy",
                Parameters = new Dictionary<string, string> { { "amount", "100" } },
                InvolvedEntities = new List<EntityId> { state.Countries[0].Id },
                IsRead = false,
                RequiresDecision = false
            });
        }
        return state;
    }
    
    private GameState ProcessEvents(GameState state) => state;
    private GameState ProcessSuccession(GameState state) => state;
}
