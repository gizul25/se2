using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using SE2.Data;
using SE2.Domain;
using SE2.Models;

namespace SE2.ViewModels;

public partial class EditProductionUnitViewModel : ViewModelBase
{
    [ObservableProperty]
    private ProductionUnitsModel _selectedProductionUnit;

    [ObservableProperty]
    private bool? _canSave = false;

    [ObservableProperty]
    private string _cancelContent = "Delete unit";

    private readonly string originalName;
    private readonly int UnitIndex = -1;
    
    public event EventHandler? Redraw;

    public EditProductionUnitViewModel(ProductionUnitsModel productionUnits)
    {
        SelectedProductionUnit = productionUnits;
        originalName = productionUnits.Name;
        UnitIndex = productionUnits.UnitIndex;

        if (UnitIndex == -1)
        {
            CancelContent = "Cancel";
        }
    }

    [RelayCommand]
    public void OnDelete()
    {
        if (UnitIndex != -1)
        {
            DM.AM.Assets.RemoveAt(UnitIndex);
        }
        Redraw?.Invoke(null,new());
        DialogHost.Close("MainDialogHost");
    }

    [RelayCommand]
    public void OnSave()
    {
        if (UnitIndex != -1)
        {
            int a = DM.AM.ScenarioData.AvailableUnits.IndexOf(originalName);
            if (-1 != a) 
                DM.AM.ScenarioData.AvailableUnits[a] = SelectedProductionUnit.Name;

            int b = DM.AM.ScenarioData.AvailableMaintenanceUnits.IndexOf(originalName);
            

            if (SelectedProductionUnit.ShallMaintained)
            {
                if (-1 != b)
                {
                    DM.AM.ScenarioData.AvailableMaintenanceUnits[b] = SelectedProductionUnit.Name;
                    DM.AM.ScenarioData.MaintenanceHoursMin[b] = SelectedProductionUnit.MinHour;
                    DM.AM.ScenarioData.MaintenanceHoursMax[b] = SelectedProductionUnit.MaxHour;
                }
                else
                {
                    DM.AM.ScenarioData.AvailableMaintenanceUnits.Add(SelectedProductionUnit.Name);
                    DM.AM.ScenarioData.MaintenanceHoursMin.Add(SelectedProductionUnit.MinHour);
                    DM.AM.ScenarioData.MaintenanceHoursMax.Add(SelectedProductionUnit.MaxHour);
                }       
            }
            else
            {
                DM.AM.ScenarioData.AvailableMaintenanceUnits.Remove(originalName);
                DM.AM.ScenarioData.MaintenanceHoursMin.RemoveAt(b);
                DM.AM.ScenarioData.MaintenanceHoursMax.RemoveAt(b);
            }

            DM.AM.Assets[UnitIndex].Name = SelectedProductionUnit.Name;
            DM.AM.Assets[UnitIndex].MaxHeat = SelectedProductionUnit.MaxHeat;
            DM.AM.Assets[UnitIndex].GasConsumption = SelectedProductionUnit.GasConsumption;
            DM.AM.Assets[UnitIndex].OilConsumption = SelectedProductionUnit.OilConsumption;
            DM.AM.Assets[UnitIndex].MaxElectricity = SelectedProductionUnit.MaxElectricity;
            DM.AM.Assets[UnitIndex].Co2Emissions = SelectedProductionUnit.Co2Emissions;
            DM.AM.Assets[UnitIndex].ProductionCosts = SelectedProductionUnit.ProductionCosts;
            DM.AM.Assets[UnitIndex].ShallMaintained = SelectedProductionUnit.ShallMaintained;
            DM.AM.Assets[UnitIndex].MinHour = SelectedProductionUnit.MinHour;
            DM.AM.Assets[UnitIndex].MaxHour = SelectedProductionUnit.MaxHour;
        }
        else
        {
            DM.AM.Assets.Add(new ()
            {
                Name = SelectedProductionUnit.Name,
                MaxHeat = SelectedProductionUnit.MaxHeat,
                GasConsumption = SelectedProductionUnit.GasConsumption,
                OilConsumption = SelectedProductionUnit.OilConsumption,
                MaxElectricity = SelectedProductionUnit.MaxElectricity,
                Co2Emissions = SelectedProductionUnit.Co2Emissions,
                ProductionCosts = SelectedProductionUnit.ProductionCosts,
                ShallMaintained = SelectedProductionUnit.ShallMaintained,
                MinHour = SelectedProductionUnit.MinHour,
                MaxHour = SelectedProductionUnit.MaxHour
            });
            if (SelectedProductionUnit.ShallMaintained)
            {
                DM.AM.ScenarioData.AvailableMaintenanceUnits.Add(SelectedProductionUnit.Name);
                DM.AM.ScenarioData.MaintenanceHoursMin.Add(SelectedProductionUnit.MinHour);
                DM.AM.ScenarioData.MaintenanceHoursMax.Add(SelectedProductionUnit.MaxHour);
            }
                
        }
        Redraw?.Invoke(null,new());
        DialogHost.Close("MainDialogHost");
    }

    public void TestSave()
    {
        if (SelectedProductionUnit == null || string.IsNullOrEmpty(SelectedProductionUnit.Name))
        {
            CanSave = false;
            return;
        }

        if (originalName == SelectedProductionUnit.Name)
        {
            CanSave = true;
            return;
        }

        foreach (Asset asset in DM.AM.Assets)
        {
            if (asset.Name == SelectedProductionUnit.Name)
            {   
                CanSave = false;
                return;
            }
        }
        
        CanSave = true;
        return;
    }
}
