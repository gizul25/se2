using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SE2.Data;
using SE2.Domain;

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
            if (value && !DM.AM.ScenarioData.AvailableUnits.Contains(Name))
            {
                DM.AM.ScenarioData.AvailableUnits.Add(Name);
                DM.Load();
            }
            else if(value == false)
            {
                DM.AM.ScenarioData.AvailableUnits.Remove(Name);
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
        IsSelected = false;
    }

    [RelayCommand]
    public void OpenEditUnitMenu()
    {
        OpenEditUnit?.Invoke(this, new());
    }
}