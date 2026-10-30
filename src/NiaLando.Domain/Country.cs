using System.Text.Json.Serialization;

namespace NiaLando.Domain;

/// <summary>
/// País: 1+ províncias, capital, tesouro (ouro), exércitos, frotas, relações, líder, povo dominante.
/// Países podem desaparecer; os povos continuam, "sem Estado".
/// </summary>
public sealed class Country
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Povo dominante (etnia/cultura).</summary>
    public EntityId PrimaryPeopleId { get; set; }

    /// <summary>Capital (província).</summary>
    public EntityId CapitalId { get; set; }

    /// <summary>Tesouro (ouro).</summary>
    public double Treasury { get; set; }

    /// <summary>Índices das províncias que este país possui.</summary>
    public List<EntityId> ProvinceIds { get; set; } = new();

    /// <summary>Índice do líder atual.</summary>
    public EntityId? LeaderId { get; set; }

    /// <summary>Estabilidade (0 a 100).</summary>
    public int Stability { get; set; } = 50;

    /// <summary>Prestígio (0 a 100).</summary>
    public int Prestige { get; set; } = 50;

    /// <summary>Inflação (0 a 1).</summary>
    public double Inflation { get; set; } = 0.0;

    /// <summary>Dívida (ouro).</summary>
    public double Debt { get; set; } = 0.0;

    /// <summary>True se este país está ativo (não foi conquistado/extinto).</summary>
    public bool Active { get; set; } = true;

    /// <summary>True se este país é o país do jogador.</summary>
    public bool IsPlayer { get; set; }
}