using System;
using System.Collections.Generic;

namespace SE2.Models;
public class ProductionUnitsModel
{
    public string Name{get; set;}
    public int NetCost{get; set;}
    public int HeatCost{get; set;}
    public int ElectricityCost{get; set;}
    public int ElectricitySales{get; set;}
    public bool IsSelected{get; set;}
    public int Emissions{get; set;}
    

    public ProductionUnitsModel(string name, int netCost = 0, int heatCost = 0, int eCost = 0, int eSales = 0)
    {
        Name = name;
        NetCost = netCost;
        HeatCost = heatCost;
        ElectricityCost = eCost;
        ElectricitySales = eSales;
    }
}