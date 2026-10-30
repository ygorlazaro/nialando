namespace NiaLando.Domain;

public sealed class Army
{
    public EntityId Id { get; set; }
    public EntityId CountryId { get; set; }
    public EntityId? ProvinceId { get; set; }
    public int Size { get; set; }
}
