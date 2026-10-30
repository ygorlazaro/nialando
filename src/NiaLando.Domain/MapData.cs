namespace NiaLando.Domain;

/// <summary>
/// Dados do mapa: geometria, adjacências e raster de IDs.
/// O raster é uma grade determinística (width, height, ushort[]) em que cada pixel
/// guarda o ID da província ou zona marítima. A hit-testing (clique → província) usa
/// essa grade na própria biblioteca, testável sem Godot.
/// </summary>
public sealed class MapData
{
    /// <summary>Largura da grade (pixels).</summary>
    public int Width { get; set; }

    /// <summary>Altura da grade (pixels).</summary>
    public int Height { get; set; }

    /// <summary>Tamanho do mapa em unidades (ex.: 1000).</summary>
    public double Size { get; set; }

    /// <summary>Grade de IDs: cada pixel guarda o ID da província ou zona marítima (0 = mar sem zona).</summary>
    public ushort[] Raster { get; set; } = Array.Empty<ushort>();

    /// <summary>Províncias (células de terra).</summary>
    public List<Province> Provinces { get; set; } = new();

    /// <summary>Zonas marítimas.</summary>
    public List<MaritimeZone> MaritimeZones { get; set; } = new();

    /// <summary>Versão do gerador de mapa.</summary>
    public GeneratorVersion GeneratorVersion { get; set; } = GeneratorVersion.Current;
}