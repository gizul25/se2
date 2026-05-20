using System.Text.Json;

namespace SE2.Data;

public class AM
{
    public List<Asset> Assets { get; set; } = [];
    public Grid? HeatingGrid { get; set; }
    public ScenarioData ScenarioData { get; set; } = new();
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

    public List<Asset> GetMaintainableAssets()
    {
        List<Asset> maintainableAssets = [];
        foreach (var a in Assets)
        {
            if (ScenarioData.AvailableMaintenanceUnits.Contains(a.Name))
            {
                maintainableAssets.Add(a);
            }
        }
        return maintainableAssets;
    }
}