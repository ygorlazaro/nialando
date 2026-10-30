namespace NiaLando.Domain;

public sealed class Leader
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; } = 30;
    public LeaderAttributes Attributes { get; set; } = new();
}
