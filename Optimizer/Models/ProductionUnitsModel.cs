using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SE2.Models;

public class ProductionUnitsModel : INotifyPropertyChanged
{
    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    public string Name { get; set; }
    public Tuple<int, int> NetCost { get; set; }
    public Tuple<int, int> HeatCost { get; set; }
    public Tuple<int, int> ElectricityCost { get; set; }
    public Tuple<int, int> ElectricitySales { get; set; }
    public int Emissions { get; set; }


    public ProductionUnitsModel(string name, int netCost = 0, int heatCost = 0, int eCost = 0, int eSales = 0)
    {
        Name = name;
        NetCost = Tuple.Create(netCost, 0); //Item1 - Normal Value, Item2 - Optimized Value
        HeatCost = Tuple.Create(heatCost, 0);
        ElectricityCost = Tuple.Create(eCost, 0);
        ElectricitySales = Tuple.Create(eSales, 0);
        IsSelected = false;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}