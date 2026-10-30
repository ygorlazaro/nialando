using Godot;
using NiaLando.Application;
using NiaLando.Simulation;
using NiaLando.Domain;

namespace NiaLando.Game.Map;

public partial class MapViewer : Node2D
{
    private Sprite2D? _mapSprite;

    public override void _Ready()
    {
        _mapSprite = new Sprite2D();
        AddChild(_mapSprite);
        
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
                ushort provId = state.Map.ProvinceIdRaster.Length > idx ? state.Map.ProvinceIdRaster[idx] : (ushort)0;
                if (provId == 0)
                    image.SetPixel(x, y, new Color(0.2f, 0.4f, 0.6f));
                else
                    image.SetPixel(x, y, new Color(0.6f, 0.8f, 0.4f));
            }
        }
        var texture = ImageTexture.CreateFromImage(image);
        _mapSprite.Texture = texture;
        _mapSprite.Position = new Vector2(512, 512);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.Pressed)
        {
            var cam = GetParent().GetNode<Camera2D>("Camera2D");
            if (mb.ButtonIndex == MouseButton.WheelUp)
                cam.Zoom *= 1.1f;
            else if (mb.ButtonIndex == MouseButton.WheelDown)
                cam.Zoom *= 0.9f;
        }
        if (@event is InputEventMouseMotion mm && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            var cam = GetParent().GetNode<Camera2D>("Camera2D");
            cam.Position -= mm.Relative / cam.Zoom;
        }
    }
}
