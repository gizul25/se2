using System;
using System.Collections.Generic;

namespace SE2.Models;

public class ProductionUnitsModel
{
    public string Name { get; set; }
    public (int valueNormal, int valueOptimized) NetCost { get; set; }
    public (int valueNormal, int valueOptimized) HeatCost { get; set; }
    public (int valueNormal, int valueOptimized) ElectricityCost { get; set; }
    public (int valueNormal, int valueOptimized) ElectricitySales { get; set; }
    public bool IsSelected { get; set; }
    public int Emissions { get; set; }


    public ProductionUnitsModel(string name, int netCost = 0, int heatCost = 0, int eCost = 0, int eSales = 0)
    {
        Name = name;
        NetCost = (netCost, 0);
        HeatCost = (heatCost, 0);
        ElectricityCost = (eCost, 0);
        ElectricitySales = (eSales, 0);
        Console.WriteLine(NetCost.GetType());
    }
}