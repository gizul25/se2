using System.Text.Json;

namespace SE2.Data;
public class AM
{
    public List<Asset> Assets { get; set; } = [];
    public Grid? HeatingGrid { get; set; }
    public ScenarioData ScenarioData { get; set; }
    private ScenarioLoader scenarioLoader = new();

    public void Load()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
        StreamReader r = new(Path.Combine(path, "Assets", "AM_production_units.json"));
        string assets = r.ReadToEnd();
        Assets = JsonSerializer.Deserialize<List<Asset>>(assets) ?? [];

        r = new StreamReader(Path.Combine(path, "Assets", "AM_heating_grid.json"));
        string grid = r.ReadToEnd();
        HeatingGrid = JsonSerializer.Deserialize<Grid>(grid);
    }

    public void LoadScenario(string scenarioName)
    {
        ScenarioData = scenarioLoader.Load(scenarioName);
    }

    public Asset? GetAssetByName(string name)
    {
        foreach (Asset asset in Assets)
        {
            if (asset.Name == name)
            {
                return asset;
            }
        }
        return null;
    }
}


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

    public override string ToString()
    {
        return $"{Name},{MaxHeat},{MaxElectricity},{ProductionCosts},{Co2Emissions},{GasConsumption},{OilConsumption}";
    }
}

public class Grid
{
    public required string Image { get; set; }
    public required string City { get; set; }
    public int Size { get; set; }
    public required string Architecture { get; set; }

    public override string? ToString()
    {
        return $"{City},{Size},{Architecture}";
    }
}