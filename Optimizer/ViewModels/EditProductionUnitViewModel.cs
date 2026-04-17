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
    private bool? _canSave = true;

    [ObservableProperty]
    private string _cancelContent = "Delete unit";

    private ProductionUnitsModel originalUnit;
    
    public event EventHandler? Redraw;

    public EditProductionUnitViewModel(ProductionUnitsModel productionUnits)
    {
        SelectedProductionUnit = productionUnits;
        originalUnit = productionUnits;

        if (productionUnits.UnitIndex == -1)
        {
            CancelContent = "Cancel";
        }
    }

    [RelayCommand]
    public void OnDelete()
    {
        if (originalUnit.UnitIndex != -1)
        {
            DM.AM.Assets.RemoveAt(originalUnit.UnitIndex);
        }
        Redraw?.Invoke(null,new());
        DialogHost.Close("MainDialogHost");
    }

    [RelayCommand]
    public void OnSave()
    {
        if (originalUnit.UnitIndex != -1)
        {
            DM.AM.Assets[originalUnit.UnitIndex].Name = SelectedProductionUnit.Name;
            DM.AM.Assets[originalUnit.UnitIndex].MaxHeat = SelectedProductionUnit.MaxHeat;
            DM.AM.Assets[originalUnit.UnitIndex].GasConsumption = SelectedProductionUnit.GasConsumption;
            DM.AM.Assets[originalUnit.UnitIndex].OilConsumption = SelectedProductionUnit.OilConsumption;
            DM.AM.Assets[originalUnit.UnitIndex].MaxElectricity = SelectedProductionUnit.MaxElectricity;
            DM.AM.Assets[originalUnit.UnitIndex].Co2Emissions = SelectedProductionUnit.Co2Emissions;
            DM.AM.Assets[originalUnit.UnitIndex].ProductionCosts = SelectedProductionUnit.ProductionCosts;
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
                ProductionCosts = SelectedProductionUnit.ProductionCosts
            });
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

        if (originalUnit.Name == SelectedProductionUnit.Name)
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
