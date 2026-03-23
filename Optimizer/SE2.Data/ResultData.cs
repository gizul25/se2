using System;

namespace SE2.Data;

public class ResultData
{
    public DateTime Time = new();
    public float HeatProduction;
    public decimal Costs;
    public float Consumption;
    public float Emissions;

    public override string? ToString()
    {
        return $"{Time} {HeatProduction} {Costs} {Consumption} {Emissions}";
    }
}