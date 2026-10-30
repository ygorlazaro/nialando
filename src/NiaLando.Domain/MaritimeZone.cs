namespace NiaLando.Domain;

/// <summary>
/// Zona marítima (agrupamento de células de mar). Tem portos associados (províncias costeiras).
/// </summary>
public sealed class MaritimeZone
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Ídices das células de mar que compõem esta zona.</summary>
    public List<int> CellIndices { get; set; } = new();

    /// <summary>Províncias costeiras que têm porto nesta zona.</summary>
    public List<EntityId> PortProvinces { get; set; } = new();

    /// <summary>Índices das províncias costeiras vizinhas.</summary>
    public List<EntityId> CoastalProvinces { get; set; } = new();

    /// <summary>Índices das zonas marítimas vizinhas.</summary>
    public List<EntityId> Neighbours { get; set; } = new();
}