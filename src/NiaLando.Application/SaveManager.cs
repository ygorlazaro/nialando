using System.Text.Json;
using NiaLando.Domain;

namespace NiaLando.Application;

public static class SaveManager
{
    private static string GetSaveDirectory()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NiaLando", "saves");
        Directory.CreateDirectory(dir);
        return dir;
    }

    public static void Save(GameState state, string slotName)
    {
        var path = Path.Combine(GetSaveDirectory(), $"{slotName}.json");
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(path, json);
    }

    public static GameState Load(string slotName)
    {
        var path = Path.Combine(GetSaveDirectory(), $"{slotName}.json");
        var json = File.ReadAllText(path);
        var state = JsonSerializer.Deserialize<GameState>(json);
        return state ?? throw new InvalidOperationException("Falha ao carregar save");
    }

    public static string[] ListSlots()
    {
        var dir = GetSaveDirectory();
        if (!Directory.Exists(dir)) return Array.Empty<string>();
        return Directory.GetFiles(dir, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => n != null)
            .Cast<string>()
            .ToArray();
    }
}
