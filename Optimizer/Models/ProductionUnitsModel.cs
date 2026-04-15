using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SE2.Data;
using SE2.Domain;
using SE2.ViewModels;
using SE2.Views;

namespace SE2.Models;

public partial class ProductionUnitsModel : Asset
{
    private bool _isSelected;

    public int UnitIndex { get; set; } = -1;

    public event EventHandler? OpenEditUnit;
    
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            if (value && !DM.SelectedAssetNames.Contains(Name))
            {
                DM.SelectedAssetNames.Add(Name);
            }
            else
            {
                DM.SelectedAssetNames.Remove(Name);
            }
        }
    }

    public ProductionUnitsModel()
    {
        IsSelected = false;
    }

    [RelayCommand]
    public void OpenEditUnitMenu()
    {
        OpenEditUnit?.Invoke(this, new());
    }
}