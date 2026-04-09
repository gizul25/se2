using System.Text.Json.Serialization;

namespace SE2.Data;

public class ResultData
{
    [JsonPropertyName("result_rows")]
    public List<ResultRow> ResultRows { get; set; } = [];

    [JsonPropertyName("scheduler_rows")]
    public List<SchedulerRow> SchedulerRows { get; set; } = [];
}

public class ResultRow
{
    [JsonPropertyName("time")]
    public DateTime Time { get; set; } = new();

    [JsonPropertyName("heat_production")]
    public double HeatProduction { get; set; }

    [JsonPropertyName("costs")]
    public decimal Costs { get; set; }

    [JsonPropertyName("consumption")]
    public double Consumption { get; set; }

    [JsonPropertyName("emissions")]
    public double Emissions { get; set; }
    
    public override string? ToString()
    {
        return $"{Time} {HeatProduction} {Costs} {Consumption} {Emissions}";
    }
}

public class SchedulerRow
{
    [JsonPropertyName("time")]
    public DateTime Time { get; set; } = new();

    [JsonPropertyName("asset")]
    public string AssetName { get; set; }

    [JsonPropertyName("heat_production")]
    public double HeatProduction { get; set; }

    [JsonPropertyName("costs")]
    public decimal Costs { get; set; }

    [JsonPropertyName("consumption")]
    public double Consumption { get; set; }

    [JsonPropertyName("emissions")]
    public double Emissions { get; set; }
    
    public override string? ToString()
    {
        return $"{Time} {AssetName} {HeatProduction} {Costs} {Consumption} {Emissions}";
    }
}