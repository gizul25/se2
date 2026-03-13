using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;


public class AM
{
    public List<Asset> Assets = new List<Asset>();
    public Grid HeatingGrid = new Grid();

    public void Load(string path)
    {
        StreamReader r = new StreamReader(Path.Combine(path, "Assets", "AM_production_units.json"));
        string assets = r.ReadToEnd();
        Assets = JsonSerializer.Deserialize<List<Asset>>(assets);
        r = new StreamReader(Path.Combine(path, "Assets", "AM_heating_grid.json"));
        string grid = r.ReadToEnd();
        HeatingGrid = JsonSerializer.Deserialize<Grid>(grid);
    }

}


public class Asset
{
    public required string Name { get; set; }
    public float MaxHeat { get; set; }
    public int ProductionCosts { get; set; }
    public int? Co2Emissions { get; set; }
    public float? GasConsumption { get; set; }
    public float? MaxElectricity { get; set; }
    public float? OilConsumption { get; set; }
    public string Image { get; set; }

    public override string? ToString()
    {
        return $"{Name},{MaxHeat},{MaxElectricity},{ProductionCosts},{Co2Emissions},{GasConsumption},{OilConsumption}";
    }
}

public class Grid
{
    public string Image { get; set; }
    public string City { get; set; }
    public int Size { get; set; }
    public string Architecture { get; set; }

    public override string? ToString()
    {
        return $"{City},{Size},{Architecture}";
    }
}