using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace SE2.ViewModels;

public partial class ConfigurationViewModel : ViewModelBase
{

    [ObservableProperty]
    private List<string> _productionUnits =
    [
        "Unit 1",
        "Unit 2",
        "Unit 3"
    ];

    [ObservableProperty]
    private string? _selectedProductionUnit;


    [ObservableProperty]
    private int _maintenanceFrom;

    [ObservableProperty]
    private int _maintenanceTo;


    [ObservableProperty]
    private List<string> _priorities =
    [
        "Boiler first",
        "Heat pump first",
        "Cheapest first"
    ];

    [ObservableProperty]
    private string? _selectedPriority;


    [RelayCommand]
    private void SaveConfiguration()
    {
        System.Console.WriteLine("=== CONFIGURATION ===");
        System.Console.WriteLine($"Unit: {SelectedProductionUnit}");
        System.Console.WriteLine($"Maintenance: {MaintenanceFrom} - {MaintenanceTo}");
        System.Console.WriteLine($"Priority: {SelectedPriority}");
    }

    public void Load()
    {
        SelectedProductionUnit = ProductionUnits.Count > 0 ? ProductionUnits[0] : null;
        SelectedPriority = Priorities[0];

        MaintenanceFrom = 0;
        MaintenanceTo = 24;
    }
}