namespace NiaLando.Domain;

public sealed class DiplomaticRelation
{
    public EntityId CountryAId { get; set; }
    public EntityId CountryBId { get; set; }
    public int Opinion { get; set; } = 0;
    public List<string> Treaties { get; set; } = new();
}

public sealed class Treaty
{
    public string Type { get; set; } = string.Empty; // alliance, non_aggression, guarantee, trade, royal_marriage, vassal, tributary, military_access
    public EntityId FromId { get; set; }
    public EntityId ToId { get; set; }
    public int TurnsRemaining { get; set; }
}
