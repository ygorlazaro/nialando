using Godot;
using NiaLando.Simulation;
using NiaLando.Domain;

namespace NiaLando.Game.Map;

public partial class MapViewer : Node2D
{
    public override void _Ready()
    {
        GD.Print("MAPVIEWER ALIVE");
        var sprite = GetNode<Sprite2D>("MapSprite");
        GD.Print("SPRITE OK");
        
        var generator = new MapGenerator();
        var setup = new SetupGenerator();
        var state = generator.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 200 });
        state = setup.Setup(state, 8);
        
        var image = Image.CreateEmpty(512, 512, false, Image.Format.Rgb8);
        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                int idx = y * 512 + x;
                ushort provId = idx < state.Map.ProvinceIdRaster.Length ? state.Map.ProvinceIdRaster[idx] : (ushort)0;
                if (provId == 0)
                    image.SetPixel(x, y, Colors.Blue);
                else
                    image.SetPixel(x, y, Colors.Green);
            }
        }
        var tex = ImageTexture.CreateFromImage(image);
        sprite.Texture = tex;
        sprite.Visible = true;
        sprite.Centered = true;
        GD.Print("MAPVIEWER DONE");
    }
}
