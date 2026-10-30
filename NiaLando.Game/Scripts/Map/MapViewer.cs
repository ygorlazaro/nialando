using Godot;
using NiaLando.Domain;
using NiaLando.Simulation;
using NiaLando.Game.UI;

namespace NiaLando.Game.Map;

public partial class MapViewer : Node2D
{
    private Sprite2D _sprite = null!;
    private Image _image = null!;
    private ImageTexture _texture = null!;
    private Camera2D _camera = null!;
    private GameState _state = null!;
    private ushort[] _raster = Array.Empty<ushort>();
    private int _width;
    private int _height;
    private bool _politicalMode;

    private Vector2 _dragStart;
    private Vector2 _cameraOffsetStart;
    private bool _dragging;

    public override void _Ready()
    {
        _sprite = new Sprite2D { Centered = false };
        AddChild(_sprite);

        _camera = GetNodeOrNull<Camera2D>("../Camera2D");
        if (_camera == null)
        {
            _camera = new Camera2D();
            AddChild(_camera);
        }

        var generator = new MapGenerator();
        _state = generator.Generate(new MapParams
        {
            Seed = 12345,
            NumProvinciasAlvo = 200,
            PorcentagemTerra = 0.45,
            Width = 512,
            Height = 512
        });

        _width = _state.Map.Width;
        _height = _state.Map.Height;
        _raster = _state.Map.ProvinceIdRaster;

        _image = Image.CreateEmpty(_width, _height, false, Image.Format.Rgba8);
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _image.SetPixel(x, y, PixelColor(x, y));
            }
        }
        _texture = ImageTexture.CreateFromImage(_image);
        _sprite.Texture = _texture;

        Redraw();

        SetProcessInput(true);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Right && mb.Pressed)
            {
                _dragging = true;
                _dragStart = GetViewport().GetMousePosition();
                _cameraOffsetStart = _camera.Offset;
                GetViewport().SetInputAsHandled();
                return;
            }

            if (mb.ButtonIndex == MouseButton.Right && !mb.Pressed)
            {
                _dragging = false;
                GetViewport().SetInputAsHandled();
                return;
            }

            if (mb.ButtonIndex == MouseButton.Left && mb.Pressed && !_dragging)
            {
                SelectProvinceAt(GetViewport().GetMousePosition());
                GetViewport().SetInputAsHandled();
                return;
            }

            if (mb.ButtonIndex == MouseButton.WheelUp)
            {
                _camera.Zoom *= 1.2f;
                GetViewport().SetInputAsHandled();
                return;
            }

            if (mb.ButtonIndex == MouseButton.WheelDown)
            {
                _camera.Zoom /= 1.2f;
                GetViewport().SetInputAsHandled();
                return;
            }
        }

        if (@event is InputEventKey ek && ek.Pressed && ek.Keycode == Key.P)
        {
            _politicalMode = !_politicalMode;
            Redraw();
            GetViewport().SetInputAsHandled();
        }
    }

    public override void _Process(double delta)
    {
        if (!_dragging) return;
        var mouse = GetViewport().GetMousePosition();
        var deltaPos = mouse - _dragStart;
        _camera.Offset = _cameraOffsetStart - deltaPos;
    }

    private void Redraw()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _image.SetPixel(x, y, PixelColor(x, y));
            }
        }
        _texture.Update(_image);
    }

    private Color PixelColor(int x, int y)
    {
        ushort id = _raster[y * _width + x];
        if (id == 0) return new Color(0.08f, 0.28f, 0.62f, 1f); // oceano

        var prov = _state.Map.Provinces.FirstOrDefault(p => p.Id.Value == id);
        if (prov == null) return new Color(0.3f, 0.3f, 0.3f, 1f);

        if (_politicalMode) return ProvinceColor(id);

        return prov.Terrain switch
        {
            Terrain.Plains => new Color(0.42f, 0.72f, 0.32f, 1f),
            Terrain.Forest => new Color(0.22f, 0.52f, 0.22f, 1f),
            Terrain.Hills => new Color(0.62f, 0.56f, 0.42f, 1f),
            Terrain.Mountains => new Color(0.56f, 0.56f, 0.62f, 1f),
            Terrain.Marsh => new Color(0.40f, 0.66f, 0.56f, 1f),
            Terrain.Coast => new Color(0.90f, 0.80f, 0.50f, 1f),
            _ => new Color(0.42f, 0.72f, 0.32f, 1f),
        };
    }

    private static Color ProvinceColor(ushort id)
    {
        uint h = (uint)(id * 2654435761u);
        float hue = (h % 1000) / 1000f;
        return Color.FromHsv(hue, 0.55f, 0.95f, 1f);
    }

    private void SelectProvinceAt(Vector2 screenPos)
    {
        var viewport = GetViewport();
        var worldPos = viewport.GetCanvasTransform().AffineInverse() * screenPos;
        int x = (int)MathF.Round(worldPos.X);
        int y = (int)MathF.Round(worldPos.Y);
        if (x < 0 || x >= _width || y < 0 || y >= _height) return;

        ushort id = _raster[y * _width + x];
        if (id == 0)
        {
            ShowProvince(null);
            return;
        }
        var prov = _state.Map.Provinces.FirstOrDefault(p => p.Id.Value == id);
        ShowProvince(prov);
    }

    private void ShowProvince(Province? province)
    {
        var gui = GetNodeOrNull<GameUI>("../CanvasLayer/GameUI");
        gui?.ShowProvince(province);
    }
}
