namespace NiaLando.Domain;

public sealed class Message
{
    public int Id { get; set; }
    public int Turn { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Importance { get; set; } = 1;
    public string Key { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public List<EntityId> InvolvedEntities { get; set; } = new();
    public bool IsRead { get; set; } = false;
    public bool RequiresDecision { get; set; } = false;
}
