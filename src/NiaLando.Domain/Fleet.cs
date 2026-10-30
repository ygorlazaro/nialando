namespace NiaLando.Domain;

public sealed class Fleet
{
    public EntityId Id { get; set; }
    public EntityId CountryId { get; set; }
    public EntityId? MaritimeZoneId { get; set; }
    public int Size { get; set; }
}
