namespace SE2.Data;

public class Asset
{
    public required string Name { get; set; }
    public float MaxHeat { get; set; }
    public int ProductionCosts { get; set; }
    public int Co2Emissions { get; set; }
    public float GasConsumption { get; set; }
    public float MaxElectricity { get; set; }
    public float OilConsumption { get; set; }
    public string? Image { get; set; }
    public DateTime? MaintananceStart { get; set; }
    public DateTime? MaintananceEnd { get; set; }
    public bool ShallMaintained { get; set; }
    public int MinHour { get; set; } = 30;
    public int MaxHour { get; set; } = 60;
    public string Color { get; set; }

    public override string ToString()
    {
        return $"{Name},{MaxHeat},{MaxElectricity},{ProductionCosts},{Co2Emissions},{GasConsumption},{OilConsumption},{ShallMaintained}";
    }
}