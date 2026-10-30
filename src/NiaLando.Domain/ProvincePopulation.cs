namespace NiaLando.Domain;

/// <summary>
/// População de um povo em uma província.
/// </summary>
public sealed class ProvincePopulation
{
    public EntityId PeopleId { get; set; }
    public EntityId ProvinceId { get; set; }
    public int Count { get; set; }
}