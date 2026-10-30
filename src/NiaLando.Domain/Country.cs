namespace NiaLando.Domain;

public sealed class Country
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EntityId? CapitalProvinceId { get; set; }
    public EntityId? RulerId { get; set; }
    public EntityId DominantPeopleId { get; set; }
    public long Treasury { get; set; }
}
