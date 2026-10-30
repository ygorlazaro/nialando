namespace NiaLando.Domain;

/// <summary>
/// Exército terrestre. Tamanho, moral, qualidade, general, terreno, cercio, suprimento.
/// </summary>
public sealed class Army
{
    public EntityId Id { get; set; }
    public EntityId OwnerId { get; set; }
    public EntityId ControllerId { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Tamanho (número de homens).</summary>
    public int Size { get; set; }

    /// <summary>Moral (0 to 1).</summary>
    public double Morale { get; set; } = 1.0;

    /// <summary>Qualidade (0 to 1).</summary>
    public double Quality { get; set; } = 0.5;

    /// <summary>Localização (índice da província).</summary>
    public EntityId LocationId { get; set; }

    /// <summary>Índice do general (líder). Pode ser nulo se for um exército sem general.</summary>
    public EntityId? GeneralId { get; set; }

    /// <summary>True se está sob cercio.</summary>
    public bool UnderSiege { get; set; }

    /// <summary>Turnos de recrutamento restantes (0 se já está no campo).</summary>
    public int RecruitmentTurnsRemaining { get; set; }
}