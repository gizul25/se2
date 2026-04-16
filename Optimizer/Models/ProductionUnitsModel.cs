using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using SE2.Domain;

namespace SE2.Models;

public partial class ProductionUnitsModel : SE2.Data.Asset, INotifyPropertyChanged
{
    private bool _isSelected;

    public decimal NetCostNormal { get; set; } = 0m;
    public decimal NetCostOptimized { get; set; } = 0m;
    public decimal HeatCostNormal { get; set; } = 0m;
    public decimal HeatCostOptimized { get; set; } = 0m;
    public decimal ElectricityCostNormal { get; set; } = 0m;
    public decimal ElectricityCostOptimized { get; set; } = 0m;
    public decimal ElectricitySalesNormal { get; set; } = 0m;
    public decimal ElectricitySalesOptimized { get; set; } = 0m;

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
            else if (!value)
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

    [SetsRequiredMembers]
    public ProductionUnitsModel(string name)
    {
        Name = name;
        Image = string.Empty;
        IsSelected = false;
    }
}