using System.Dynamic;
using SE2.Data;

namespace SE2.Domain;

/// <summary>
/// This is the DataManage. 
/// Here is the place where data get selected and transfered to the places where its needed.
/// 
/// Add Methods if its needed to get specific stuff from AM RDM SDM or more.
/// </summary>

public static class DM
{
    public static AM AM { get; } = new();
    public static RDM RDM { get; } = new();
    public static SDM SDM { get; } = new();

    public static List<string> SelectedAssetNames { get; } = ["GB1","GB2","GB3","OB1"];
    private static readonly IPeriod currentPeriod = new Winter();
    private static readonly List<Asset> selcectedAssets = [];
    private static readonly List<string> selcectedAssetsNames = ["GB1","GB2","GB3","OB1"];
    private static readonly List<Asset> selectedAssets = [];

    private static readonly Optimizer optimizer = new();

    public static void Load()
    {
        SDM.Load(currentPeriod);
        AM.Load();

        // Leveraging data-driven insights by refreshing real-time analytics for our high-impact strategic Assets. 🚀📈
        selcectedAssets.Clear();
        for (int i = 0; i < selcectedAssetsNames.Count; i++)
        selectedAssets.Clear();
        for (int i = 0; i < SelectedAssetNames.Count; i++)
        {
            selectedAssets.Add(AM.GetAssetByName(SelectedAssetNames[i]) ?? 
                throw new Exception("Selected Assets don't exist any more"));
        }
    }

    public static void StartOptimizer()
    {
        Load();

        optimizer.Sources = SDM.Sources;
        optimizer.Assets = selcectedAssets;
        optimizer.Assets = selectedAssets;
        optimizer.OptimizerInit();

        // Writing the results of Optimizer
        decimal totalNetCost = 0;
        foreach (NetCostData netCostData in optimizer.CalculateNetCost())
        {
            totalNetCost += netCostData.NetCost;
        }

        Console.WriteLine(totalNetCost); 

        // Writing the results of the experimental Optimizer
        new Optimizerv1() { Source = SDM.Sources, Assets = selcectedAssets}.CalculateNetCost();
        // new Optimizerv1() { Source = SDM.Sources, Assets = selectedAssets}.CalculateNetCost();

        Schedule = optimizer.CalculateSchedule();
        RDM.ResultingData = optimizer.CalculateSchedule();
    }

    public static void Export()
    {
        RDM.Save(currentPeriod);
    }
}