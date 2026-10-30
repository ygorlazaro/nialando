using Godot;
using NiaLando.Simulation;
using NiaLando.Domain;
using NiaLando.Game.UI;

namespace NiaLando.Game.Map;

public partial class MapViewer : Node2D
{
    private Sprite2D? _mapSprite;
    private GameState? _state;
    private GameUI? _gameUI;

    public override void _Ready()
    {
        _mapSprite = GetNode<Sprite2D>("MapSprite");
        _gameUI = GetParent().GetNodeOrNull<GameUI>("CanvasLayer/GameUI");
        
        var generator = new MapGenerator();
        var setup = new SetupGenerator();
        _state = generator.Generate(new MapParams { Seed = 12345, NumProvinciasAlvo = 200 });
        _state = setup.Setup(_state, 8);
        
        GD.Print($"[MapViewer] Provincias: {_state.Map.Provinces.Count}");
        int ocean = 0, land = 0;
        for (int i = 0; i < _state.Map.ProvinceIdRaster.Length; i++)
        {
            if (_state.Map.ProvinceIdRaster[i] == 0) ocean++; else land++;
        }
        GD.Print($"[MapViewer] Ocean:{ocean} Land:{land}");
        
        var image = Image.CreateEmpty(512, 512, false, Image.Format.Rgb8);
        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                int idx = y * 512 + x;
                ushort provId = idx < _state.Map.ProvinceIdRaster.Length ? _state.Map.ProvinceIdRaster[idx] : (ushort)0;
                if (provId == 0)
                    image.SetPixel(x, y, new Color(0.1f, 0.4f, 0.8f));
                else
                    image.SetPixel(x, y, new Color(0.6f, 0.8f, 0.3f));
            }
        }
        var texture = ImageTexture.CreateFromImage(image);
        _mapSprite.Texture = texture;
        _mapSprite.Visible = true;
        _mapSprite.Modulate = Colors.White;
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
            else if (mb.ButtonIndex == MouseButton.Left && _mapSprite != null && _mapSprite.Texture != null)
            {
                var mousePos = GetGlobalMousePosition();
                var localPos = mousePos - _mapSprite.GlobalPosition + new Vector2(256, 256);
                int x = (int)Mathf.Clamp(localPos.X, 0, 511);
                int y = (int)Mathf.Clamp(localPos.Y, 0, 511);
                if (_state != null && _state.Map.ProvinceIdRaster.Length > y * 512 + x)
                {
                    ushort provId = _state.Map.ProvinceIdRaster[y * 512 + x];
                    if (provId == 0)
                    {
                        _gameUI?.ShowProvince(null);
                        return;
                    }
                    var province = _state.Map.Provinces.Find(p => p.Id.Value == provId);
                    _gameUI?.ShowProvince(province);
                }
            }
        }
        if (@event is InputEventMouseMotion mm && Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            var cam = GetParent().GetNode<Camera2D>("Camera2D");
            cam.Position -= mm.Relative / cam.Zoom;
        }
    }
}
