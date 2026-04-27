using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SE2.Models;
using SE2.Data;
using SE2.Domain;
using DialogHostAvalonia;
using System.Threading.Tasks;
using SE2.Views;

namespace SE2.ViewModels;

public partial class ProductionUnitsViewModel : ViewModelBase
{

    [ObservableProperty]
    private bool? _selectAll = false;
    
    [ObservableProperty]
    public ObservableCollection<ProductionUnitsModel> _productionUnits = [];

    public ProductionUnitsViewModel()
    {
        Draw();
    }

    [ObservableProperty]
    private ProductionUnitsModel? _selectedProductionUnit = null;

    partial void OnSelectAllChanged(bool? value)
    {
        if (value.HasValue)
        {
            foreach (ProductionUnitsModel productionUnit in ProductionUnits)
            {
                productionUnit.IsSelected = value.Value;
            }
        }
    }

    [RelayCommand]
    public void OpenAddUnitMenu()
    {
        _ = OpenEditMenu(new () { Name = "Unit"});
    }

    private void Draw()
    {
        ProductionUnits = [];
        Console.WriteLine(string.Join(", ", DM.AM.ScenarioData.AvailableMaintenanceUnits));
        
        for (int index = 0; index < DM.AM.Assets.Count; index++)
        {
            Asset asset = DM.AM.Assets[index];
            ProductionUnitsModel unitsModel = new () {
                Name = asset.Name,
                ProductionCosts = asset.ProductionCosts,
                MaxHeat = asset.MaxHeat,
                Co2Emissions = asset.Co2Emissions,
                GasConsumption = asset.GasConsumption,
                MaxElectricity = asset.MaxElectricity,
                OilConsumption = asset.OilConsumption,
                ShallMaintained = DM.AM.ScenarioData.AvailableMaintenanceUnits.Contains(asset.Name),
                MaxHour = asset.MaxHour,
                MinHour = asset.MinHour,
                IsSelected = DM.AM.ScenarioData.AvailableUnits.Contains(asset.Name),
                UnitIndex = index
            };
            unitsModel.OpenEditUnit += OpenEditMenu;
            ProductionUnits.Add(unitsModel);
        }
    }

    private void Redraw(object? sender, EventArgs args)
    {
        Draw();
    }

    private void OpenEditMenu(object? sender, EventArgs args)
    {
        if (sender is ProductionUnitsModel model)
        {
            _ = OpenEditMenu(model);
        }
    }

    public async Task OpenEditMenu(ProductionUnitsModel productionUnits)
    {
        EditProductionUnitViewModel editViewModel = new(productionUnits);
        editViewModel.Redraw += Redraw;
        await DialogHost.Show(new EditProductionUnitView() {DataContext = editViewModel}, "MainDialogHost");
    }
}