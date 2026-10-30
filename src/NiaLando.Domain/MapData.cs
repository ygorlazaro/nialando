namespace NiaLando.Domain;

/// <summary>
/// Dados do mapa gerado. Geometry/adjacências/raster guardados no save.
/// </summary>
public sealed class MapData
{
    public int Width { get; set; }
    public int Height { get; set; }
    public ushort[] ProvinceIdRaster { get; set; } = Array.Empty<ushort>();
    public List<Province> Provinces { get; set; } = new();
    public List<MaritimeZone> MaritimeZones { get; set; } = new();
    public List<Region> Regions { get; set; } = new();
    public GeneratorVersion GeneratorVersion { get; set; } = GeneratorVersion.Current;
}
