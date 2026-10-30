using Godot;
using NiaLando.Simulation;
using NiaLando.Domain;

namespace NiaLando.Game.Map;

public partial class MapViewer : Node2D
{
    public override void _Ready()
    {
        GD.Print("=== MAPVIEWER READY ===");
        var sprite = GetNode<Sprite2D>("MapSprite");
        var generator = new MapGenerator();
        var state = generator.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 200 });
        GD.Print($"Provs: {state.Map.Provinces.Count}");
        var image = Image.CreateEmpty(512, 512, false, Image.Format.Rgb8);
        int land = 0;
        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                if (state.Map.ProvinceIdRaster[y * 512 + x] == 0)
                    image.SetPixel(x, y, new Color(0, 0, 1));
                else
                {
                    land++;
                    image.SetPixel(x, y, new Color(0, 1, 0));
                }
            }
        }
        GD.Print($"Land pixels: {land}");
        var tex = ImageTexture.CreateFromImage(image);
        sprite.Texture = tex;
        sprite.Centered = true;
        sprite.Position = Vector2.Zero;
        sprite.Visible = true;
        sprite.Modulate = Colors.White;
        GD.Print($"Sprite texture null? {sprite.Texture == null}");
        GD.Print("=== MAPVIEWER DONE ===");
    }
}
