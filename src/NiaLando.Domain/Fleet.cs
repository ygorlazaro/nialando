namespace NiaLando.Domain;

/// <summary>
/// Frota naval. Em zonas marítimas; bloqueia portos, transporta tropas.
/// </summary>
public sealed class Fleet
{
    public EntityId Id { get; set; }
    public EntityId OwnerId { get; set; }
    public EntityId ControllerId { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Tamanho (número de navios).</summary>
    public int Size { get; set; }

    /// <summary>Moral (0 to 1).</summary>
    public double Morale { get; set; } = 1.0;

    /// <summary>Localização (ID da zona marítima).</summary>
    public EntityId LocationId { get; set; }

    /// <summary>Índice do almirante (líder). Pode ser nulo.</summary>
    public EntityId? AdmiralId { get; set; }

    /// <summary>True se está bloqueando um porto.</summary>
    public bool Blockading { get; set; }

    /// <summary>True se está transportando tropas.</summary>
    public bool Transporting { get; set; }
}