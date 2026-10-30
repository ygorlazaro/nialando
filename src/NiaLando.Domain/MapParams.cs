using System;
using System.Collections.Generic;

namespace NiaLando.Domain;

public sealed class MapParams
{
    public double Tamanho { get; init; } = 1000.0;
    public double DistanciaMinima { get; init; } = 33.33;
    public double PorcentagemTerra { get; init; } = 0.45;
    public int IteracoesLloyd { get; init; } = 3;
    public int OctavasRuido { get; init; } = 5;
    public int NumProvinciasAlvo { get; init; } = 1000;
    public int MinTamanhoRio { get; init; } = 5;
    public int Width { get; init; } = 512;
    public int Height { get; init; } = 512;
    public long Seed { get; init; } = 0;
}
