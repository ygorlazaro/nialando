namespace NiaLando.Domain;

/// <summary>
/// Atributos do líder. Cada um de 10 a 100.
/// - Diplomacia: melhores relações, mais alianças e acordos.
/// - Violência: declara guerra mais fácil e comanda melhor exércitos.
/// - Ganância: prioriza guerras por ouro/tributo.
/// </summary>
public sealed class LeaderAttributes
{
    public int Diplomacy { get; set; } = 50;
    public int Violence { get; set; } = 50;
    public int Greed { get; set; } = 50;

    public LeaderAttributes() { }

    public LeaderAttributes(int diplomacy, int violence, int greed)
    {
        Diplomacy = Clamp(diplomacy);
        Violence = Clamp(violence);
        Greed = Clamp(greed);
    }

    private static int Clamp(int value) => Math.Clamp(value, 10, 100);

    public bool IsValid() =>
        Diplomacy >= 10 && Diplomacy <= 100 &&
        Violence >= 10 && Violence <= 100 &&
        Greed >= 10 && Greed <= 100;

    public LeaderAttributes Clone() => new(Diplomacy, Violence, Greed);
}