namespace NiaLando.Domain;

/// <summary>
/// Relação diplomática entre dois países. Opinião com modificadores (causa + duração).
/// </summary>
public sealed class DiplomaticRelation
{
    public EntityId CountryA { get; set; }
    public EntityId CountryB { get; set; }

    /// <summary>Opinião (-100 a 100).</summary>
    public int Opinion { get; set; }

    /// <summary>Aliança?</summary>
    public bool IsAllied { get; set; }

    /// <summary>Garantia de independência?</summary>
    public bool GuaranteesIndependence { get; set; }

    /// <summary>Acordo de não agressão?</summary>
    public bool NonAggressionPact { get; set; }

    /// <summary>Acordo comercial?</summary>
    public bool TradeAgreement { get; set; }

    /// <summary>Casamento real?</summary>
    public bool RoyalMarriage { get; set; }

    /// <summary>Vassalagem: CountryB é vassalo de CountryA?</summary>
    public bool IsVassal { get; set; }

    /// <summary>Tributário: CountryB paga tributo a CountryA?</summary>
    public bool IsTributary { get; set; }

    /// <summary>Acesso militar concedido por CountryB a CountryA?</summary>
    public bool MilitaryAccess { get; set; }
}