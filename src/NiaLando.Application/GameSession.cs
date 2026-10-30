using NiaLando.Domain;

namespace NiaLando.Application;

/// <summary>
/// Fachada única para acesso ao estado do jogo.
/// UI nunca altera o estado diretamente.
/// </summary>
public sealed class GameSession
{
    private GameState _state = new();

    public GameState State => _state;

    public string Title => "Nia Lando: Glavo kaj Sigelo";

    public void NewGame(MapParams? mapParams = null)
    {
        _state = new GameState
        {
            Seed = Environment.TickCount64,
            Params = mapParams ?? new MapParams(),
            GeneratorVersion = GeneratorVersion.Current,
            SaveVersion = 1,
            CurrentTurn = 0,
            CurrentYear = 1444,
            Map = new MapData(),
            Peoples = new List<People>(),
            Countries = new List<Country>(),
            Leaders = new List<Leader>(),
            Messages = new List<Message>(),
            Relations = new List<DiplomaticRelation>(),
            Armies = new List<Army>(),
            Fleets = new List<Fleet>()
        };
    }

    public void LoadGame(string path)
    {
        throw new NotImplementedException("Carregamento será implementado nas fases seguintes.");
    }

    public void SaveGame(string path)
    {
        throw new NotImplementedException("Salvamento será implementado nas fases seguintes.");
    }

    public IReadOnlyList<string> Submit(string order)
    {
        throw new NotImplementedException("Ordens serão implementadas nas fases seguintes.");
    }

    public void RevokeOrder(string orderId)
    {
        throw new NotImplementedException("Revogação será implementada nas fases seguintes.");
    }

    public void EndTurn()
    {
        throw new NotImplementedException("Fim de turno será implementado nas fases seguintes.");
    }
}
