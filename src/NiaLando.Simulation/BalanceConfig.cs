namespace NiaLando.Simulation;

public sealed class BalanceConfig
{
    public int BaseTaxIncome { get; set; } = 10;
    public int ArmyCost { get; set; } = 1000;
    public int FleetCost { get; set; } = 500;
    public double RevoltChance { get; set; } = 0.05;
}
