namespace NiaLando.Domain;

public sealed class Province
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Terrain Terrain { get; set; }
    public Climate Climate { get; set; }
    public int Development { get; set; }
    public int TradeValue { get; set; }
    public bool HasPort { get; set; }
    public int FortLevel { get; set; }
    public EntityId? OwnerId { get; set; }
    public EntityId? ControllerId { get; set; }
    public List<EntityId> AdjacentProvinceIds { get; set; } = new();
    public List<ProvincePopulation> Populations { get; set; } = new();
}
