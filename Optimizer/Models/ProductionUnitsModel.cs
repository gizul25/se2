using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SE2.Models;

public partial class ProductionUnitsModel : Asset, INotifyPropertyChanged
{
    private bool _isSelected;

    public int UnitIndex { get; set; } = -1;

    public event EventHandler? OpenEditUnit;
    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
            if (value && !DM.SelectedAssetNames.Contains(Name))
            {
                DM.SelectedAssetNames.Add(Name);
                DM.Load();
            }
            else if(value == false)
            {
                DM.SelectedAssetNames.Remove(Name);
                DM.Load();
            }
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ProductionUnitsModel()
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