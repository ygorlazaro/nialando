namespace NiaLando.Domain;

public sealed class People
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CultureStyle { get; set; } = string.Empty;
}
