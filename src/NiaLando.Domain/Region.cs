namespace NiaLando.Domain;

public sealed class Region
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<EntityId> ProvinceIds { get; set; } = new();
}
