using Godot;
using NiaLando.Simulation;
using NiaLando.Domain;

namespace NiaLando.Game.Map;

public partial class MapViewer : Node2D
{
    public override void _Ready()
    {
        CallDeferred(nameof(GenerateMap));
    }
    
    private void GenerateMap()
    {
        var sprite = GetNode<Sprite2D>("MapSprite");
        var generator = new MapGenerator();
        var state = generator.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 200 });
        var image = Image.CreateEmpty(512, 512, false, Image.Format.Rgb8);
        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                if (state.Map.ProvinceIdRaster[y * 512 + x] == 0)
                    image.SetPixel(x, y, new Color(0, 0, 1));
                else
                    image.SetPixel(x, y, new Color(0, 1, 0));
            }
        }
        var tex = ImageTexture.CreateFromImage(image);
        sprite.Texture = tex;
        sprite.Centered = true;
        sprite.Visible = true;
    }
}
