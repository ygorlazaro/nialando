namespace NiaLando.Domain;

/// <summary>
/// Versão do algoritmo de geração do mapa.
/// Incrementar Major quando o algoritmo mudar de forma incompatível.
/// O save guarda GeneratorVersion e rejeita (ou migra explicitamente) versões incompatíveis.
/// </summary>
public readonly record struct GeneratorVersion(int Major, int Minor)
{
    public static GeneratorVersion Current => new(1, 0);

    public override string ToString() => $"{Major}.{Minor}";

    public bool IsCompatibleWith(GeneratorVersion other) => Major == other.Major;
}
