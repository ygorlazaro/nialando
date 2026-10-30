using System.Text.Json.Serialization;

namespace NiaLando.Domain;

/// <summary>
/// Mensagem do jogo (ADR-0002). Dado estruturado: chave + parâmetros + entidades envolvidas.
/// A UI as renderiza via localização.
/// </summary>
public sealed class Message
{
    public int Turn { get; set; }

    /// <summary>Categoria: guerra, diplomacia, economia, povos, líderes, eventos, sistema.</summary>
    public MessageCategory Category { get; set; }

    /// <summary>Importância (0 a 100).</summary>
    public int Importance { get; set; }

    /// <summary>Chave de texto (estável, para i18n).</summary>
    public string TextKey { get; set; } = string.Empty;

    /// <summary>Parâmetros para interpolação no template de localização.</summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>Entidades envolvidas (clicáveis, centralizam o mapa).</summary>
    public List<EntityId> EntityIds { get; set; } = new();

    /// <summary>Exige decisão do jogador.</summary>
    public bool RequiresDecision { get; set; }

    /// <summary>Lida?</summary>
    public bool Read { get; set; }
}

/// <summary>
/// C categorias de mensagem.
/// </summary>
public enum MessageCategory : byte
{
    War = 0,
    Diplomacy = 1,
    Economy = 2,
    Peoples = 3,
    Leaders = 4,
    Events = 5,
    System = 6
}