using NiaLando.Domain;

namespace NiaLando.Application;

public sealed class TurnResult
{
    public GameState State { get; init; } = new();
    public List<Message> NewMessages { get; init; } = new();
    public bool Completed { get; init; }
}
