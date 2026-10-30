namespace NiaLando.Domain;

/// <summary>
/// Líder de um país. Tem idade, morre, e há sucessão (herdeiro com atributos correlacionados ao pai + ruído).
/// </summary>
public sealed class Leader
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public LeaderAttributes Attributes { get; set; } = new();
    public EntityId CountryId { get; set; }
    public int BirthYear { get; set; }
    public int Age { get; set; }
    public bool Alive { get; set; } = true;

    /// <summary>True se este líder é o líder do país do jogador.</summary>
    public bool IsPlayer { get; set; }
}