using System;

public class SourceData
{
    public DateTime StartTime = new();
    public float HeatDemand;
    public decimal ElectricityPrice;

    public override string? ToString()
    {
        return $"{StartTime} {HeatDemand} {ElectricityPrice}";
    }
}