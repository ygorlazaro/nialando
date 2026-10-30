using NiaLando.Domain;
using NiaLando.Simulation;

namespace NiaLando.Application;

/// <summary>
/// Fachada única para acesso ao estado do jogo.
/// UI nunca altera o estado diretamente.
/// </summary>
public sealed class GameSession
{
    private GameState _state = new();
    private readonly TurnProcessor _turnProcessor = new();

    public GameState State => _state;

    public string Title => "Nia Lando: Glavo kaj Sigelo";

    public void NewGame(MapParams? mapParams = null, int numCountries = 12)
    {
        var generator = new MapGenerator();
        var setup = new SetupGenerator();
        _state = generator.Generate(mapParams ?? new MapParams());
        _state = setup.Setup(_state, numCountries);
    }

    public void LoadGame(string path)
    {
        // path pode ser slot name ou caminho completo
        if (Path.IsPathRooted(path))
        {
            var json = File.ReadAllText(path);
            var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(json);
            _state = state ?? throw new InvalidOperationException("Falha ao carregar save");
        }
        else
        {
            _state = SaveManager.Load(path);
        }
    }

    public void SaveGame(string path)
    {
        if (Path.IsPathRooted(path))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_state, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        else
        {
            SaveManager.Save(_state, path);
        }
    }

    public IReadOnlyList<string> Submit(string order)
    {
        throw new NotImplementedException("Ordens serão implementadas nas fases seguintes.");
    }

    public void RevokeOrder(string orderId)
    {
        throw new NotImplementedException("Revogação será implementada nas fases seguintes.");
    }

    public TurnResult EndTurn()
    {
        var beforeCount = _state.Messages.Count;
        _state = _turnProcessor.EndTurn(_state);
        var newMessages = _state.Messages.Skip(beforeCount).ToList();
        return new TurnResult
        {
            State = _state,
            NewMessages = newMessages,
            Completed = true
        };
    }
}
