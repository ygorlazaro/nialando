namespace NiaLando.Domain;

public sealed class War
{
    public EntityId Id { get; set; }
    public EntityId AttackerId { get; set; }
    public EntityId DefenderId { get; set; }
    public int Warscore { get; set; } = 0;
    public int TurnsActive { get; set; } = 0;
    public List<string> Objectives { get; set; } = new();
    public bool IsActive { get; set; } = true;
}
