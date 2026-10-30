using System.Text.Json.Serialization;

namespace NiaLando.Domain;

/// <summary>
/// Uma província (célula de terra do mapa).
/// OwnerId e ControllerId são separados (ocupação vs. soberania).
/// </summary>
public sealed class Province
{
    public EntityId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Terrain Terrain { get; set; }
    public Climate Climate { get; set; }

    /// <summary>Desenvolvimento (0 a 100), derivado do relevo.</summary>
    public int Desenvolvimento { get; set; }

    /// <summary>Comércio (0 a 1), derivado do relevo e do porto.</summary>
    public double Comercio { get; set; }

    /// <summary>Nível do forte (0 a 3).</summary>
    public int ForteNivel { get; set; }

    /// <summary>True se esta província tem porto (terra costeira).</summary>
    public bool Porto { get; set; }

    /// <summary>Proprietário soberano (país que a possui). Pode ser nulo se for terra nadando.</summary>
    public EntityId? OwnerId { get; set; }

    /// <summary>Controlador (país que ocupa fisicamente). Pode divergir de OwnerId em caso de ocupação.</summary>
    public EntityId? ControllerId { get; set; }

    /// <summary>População por povo.</summary>
    public List<ProvincePopulation> PopulationByPeople { get; set; } = new();

    /// <summary>Geometria da célula Voronoi (vértices em coordenadas normalizadas 0..Tamanho).</summary>
    public List<(double X, double Y)> Geometry { get; set; } = new();

    /// <summary>Índices dos países vizinhos (províncias e zonas marítimas), por aresta compartilhada.</summary>
    public List<EntityId> Neighbours { get; set; } = new();
}