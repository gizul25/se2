using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SE2.Models;

namespace SE2.ViewModels;

public partial class ProductionUnitsViewModel : ViewModelBase
{
    public ObservableCollection<ProductionUnitsModel> ProductionUnits { get; set; }
    public ProductionUnitsViewModel()
    {
        ProductionUnits = new ObservableCollection<ProductionUnitsModel>(new List<ProductionUnitsModel>
        {
            new ProductionUnitsModel("Production unit 1"),
            new ProductionUnitsModel("Production unit 2"),
            new ProductionUnitsModel("Production unit 3"),
            new ProductionUnitsModel("Production unit 4"),
            new ProductionUnitsModel("Production unit 5")
        });
    }

    [ObservableProperty]
    private ProductionUnitsModel? _selectedProductionUnit = new ProductionUnitsModel("Production Unit");


}