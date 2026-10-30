namespace NiaLando.Domain;

/// <summary>
/// Identificador estável de uma entidade (província, país, povo, líder, etc.).
/// Inteiros gerados sequencialmente pelo gerador a partir do PRNG.
/// Proibido usar Guid anywhere na Simulation.
/// </summary>
public readonly record struct EntityId(int Value) : IComparable<EntityId>
{
    public int CompareTo(EntityId other) => Value.CompareTo(other.Value);
    public static implicit operator EntityId(int value) => new(value);
    public static implicit operator int(EntityId value) => value.Value;
}