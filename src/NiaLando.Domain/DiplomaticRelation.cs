namespace NiaLando.Domain;

public sealed class DiplomaticRelation
{
    public EntityId CountryAId { get; set; }
    public EntityId CountryBId { get; set; }
    public int Opinion { get; set; } = 0;
    public List<string> Treaties { get; set; } = new();
}
