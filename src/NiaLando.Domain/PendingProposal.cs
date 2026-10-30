namespace NiaLando.Domain;

public sealed class PendingProposal
{
    public int Id { get; set; }
    public EntityId FromCountryId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public bool IsSealed { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }
}
