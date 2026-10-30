namespace NiaLando.Domain;

/// <summary>
/// Povo (etnia/cultura). Independente dos países: pode estar espalhado por vários países
/// (ex.: povo basco) e um país pode ter mais de um povo.
/// </summary>
public sealed class People
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;

    /// <summary>Religião (chave de localização). Opcional.</summary>
    public string? Religion { get; set; }

    /// <summary>Índice do estilo fonético usado pelo gerador de nomes.</summary>
    public int NameStyle { get; set; }

    /// <summary>True se este povo já teve um país histórico (para irredentismo).</summary>
    public bool HadCountry { get; set; }
}