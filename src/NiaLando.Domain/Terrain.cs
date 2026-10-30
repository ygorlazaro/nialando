namespace NiaLando.Domain;

/// <summary>
/// Terreno de uma província. Valores em ordem crescente de "dureza" para o defensor.
/// </summary>
public enum Terrain : byte
{
    Pantano = 0,
    Floresta = 1,
    Costa = 2,
    Planicie = 3,
    Colinas = 4,
    Montanhas = 5
}