namespace NiaLando.Domain;

public sealed class MaritimeZone
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<EntityId> AdjacentProvinceIds { get; set; } = new();
    public bool HasPortAccess { get; set; }
}
