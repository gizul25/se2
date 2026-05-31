using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SE2.Data;

public class ResultData
{
    [JsonPropertyName("result_rows")]
    public List<ResultRow> ResultRows { get; set; } = [];

    [JsonPropertyName("scheduler_rows")]
    public List<SchedulerRow> SchedulerRows { get; set; } = [];

	[JsonPropertyName("total_revenue")]
    public double TotalRevenue { get; set; } = 0;

	[JsonPropertyName("total_expenses")]
	public double TotalExpenses { get; set; } = 0;

    [JsonPropertyName("total_profit")]
    public double TotalProfit { get; set; } = 0;

    [JsonPropertyName("total_cost")]
    public double TotalCost { get; set; } = 0;

    [JsonPropertyName("heat_produced")]
    public double HeatProduced { get; set; } = 0;

    [JsonPropertyName("electricity_consumed")]
    public double ElectricityConsumed { get; set; } = 0;

    [JsonPropertyName("electricity_produced")]
    public double ElectricityProduced { get; set; } = 0;

    [JsonPropertyName("primary_energy")]
    public double PrimaryEnergy { get; set; } = 0;

    [JsonPropertyName("co2_emissions")]
    public double Co2Emissions { get; set; } = 0;

    [JsonPropertyName("maintenance_periods")]
    public List<MaintenancePeriod> MaintenancePeriods { get; set; } = [];

    [JsonPropertyName("hourly_net_cost")]
    public List<NetCostData> HourlyNetCost { get; set; } = [];
}

public class ResultRow
{
    [JsonPropertyName("time")]
    public DateTime Time { get; set; } = new();

    [JsonPropertyName("heat_production")]
    public double HeatProduction { get; set; }

    [JsonPropertyName("costs")]
    public decimal Costs { get; set; }

    [JsonPropertyName("production")]
    public double Production { get; set; }

    [JsonPropertyName("consumption")]
    public double Consumption { get; set; }

    [JsonPropertyName("primary_energy")]
    public double PrimaryEnergy { get; set; }

    [JsonPropertyName("emissions")]
    public double Emissions { get; set; }

	[JsonPropertyName("expenses")]
    public double Expenses { get; set; }

    [JsonPropertyName("profits")]
    public double Profits { get; set; }

    public override string? ToString()
    {
        return $"{Time} {HeatProduction} {Costs} {Production} {Consumption} {PrimaryEnergy} {Emissions}";
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

    [JsonPropertyName("electricity")]
    public double Electricity { get; set; }

    [JsonPropertyName("primary_energy")]
    public double PrimaryEnergy { get; set; }

    [JsonPropertyName("emissions")]
    public double Emissions { get; set; }
	
    [JsonPropertyName("heat_expense")]
	public decimal HeatExpense { get; set; }

	[JsonPropertyName("electricity_revenue")]
	public decimal ElectricityRevenue { get; set; }

	[JsonPropertyName("electricity_expense")]
	public decimal ElectricityExpense { get; set; }

    public override string? ToString()
    {
        return $"{Time} {AssetName} {HeatProduction} {Costs} {Electricity} {PrimaryEnergy} {Emissions}";
    }
}

public class MaintenancePeriod
{
    [JsonPropertyName("maintained_unit")]
    public string MaintainedUnit { get; set; } = "";

    [JsonPropertyName("maintained_start")]
    public DateTime MaintainedStart { get; set; }

    [JsonPropertyName("maintained_end")]
    public DateTime MaintainedEnd { get; set; }
}