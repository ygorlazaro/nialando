namespace NiaLando.Domain;

/// <summary>
/// Estado serializável do jogo (grafo de dados). A Simulation é a fonte de verdade.
/// IDs inteiros estáveis; sem Guid, sem referências cíclicas, sem DateTime.Now.
/// </summary>
public sealed class GameState
{
    /// <summary>Seed do jogo (guardada no save). Determina tudo.</summary>
    public long Seed { get; set; }

    /// <summary>Parâmetros de geração.</summary>
    public MapParams Params { get; set; } = new();

    /// <summary>Versão do gerador de mapa que produziu este estado.</summary>
    public GeneratorVersion GeneratorVersion { get; set; } = GeneratorVersion.Current;

    /// <summary>Versão do formato de save.</summary>
    public int SaveVersion { get; set; } = 1;

    /// <summary>Número do turno atual (turno 0 = estado inicial do mapa).</summary>
    public int CurrentTurn { get; set; } = 0;

    /// <summary>Ano (turno 0 = 1444, por exemplo). Configurável.</summary>
    public int CurrentYear { get; set; } = 1444;

    /// <summary>País do jogador. Nulo em jogos headless/IA-only.</summary>
    public EntityId? PlayerCountryId { get; set; }

    /// <summary>Mapa: províncias, zonas marítimas e raster de IDs.</summary>
    public MapData Map { get; set; } = new();

    /// <summary>Povos.</summary>
    public List<People> Peoples { get; set; } = new();

    /// <summary>Países ativos.</summary>
    public List<Country> Countries { get; set; } = new();

    /// <summary>Líderes.</summary>
    public List<Leader> Leaders { get; set; } = new();

    /// <summary>Mensagens geradas entre os turnos.</summary>
    public List<Message> Messages { get; set; } = new();

    /// <summary>Relações entre países (opinião, tratados).</summary>
    public List<DiplomaticRelation> Relations { get; set; } = new();

    /// <summary>Exércitos ativos.</summary>
    public List<Army> Armies { get; set; } = new();

    /// <summary>Frotas ativas.</summary>
    public List<Fleet> Fleets { get; set; } = new();

    /// <summary>Guerras ativas.</summary>
    public List<War> Wars { get; set; } = new();

    /// <summary>Tratados ativos.</summary>
    public List<Treaty> Treaties { get; set; } = new();

    /// <summary>Propostas pendentes ao jogador (cartas lacradas).</summary>
    public List<PendingProposal> PendingProposals { get; set; } = new();
}
