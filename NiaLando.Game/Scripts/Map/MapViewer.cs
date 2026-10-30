using Godot;
using NiaLando.Simulation;
using NiaLando.Domain;

namespace NiaLando.Game.Map;

public partial class MapViewer : Node2D
{
    public override void _Ready()
    {
        GD.Print("MAPVIEWER START");
        var sprite = GetNode<Sprite2D>("MapSprite");
        var generator = new MapGenerator();
        var setup = new SetupGenerator();
        var state = generator.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 200 });
        state = setup.Setup(state, 8);
        GD.Print($"GOT {state.Map.Provinces.Count} PROVS");
        var image = Image.CreateEmpty(512, 512, false, Image.Format.Rgb8);
        for (int i = 0; i < 512*512; i++)
        {
            ushort provId = state.Map.ProvinceIdRaster[i];
            image.SetPixel(i % 512, i / 512, provId == 0 ? Colors.Blue : Colors.Green);
        }
        var tex = ImageTexture.CreateFromImage(image);
        sprite.Texture = tex;
        sprite.Visible = true;
        GD.Print("MAPVIEWER END");
    }
}
