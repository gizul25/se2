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
    public static List<ResultData> Schedule { get; set; } = new();

    private static readonly IPeriod currentPeriod = new Winter();
    private static readonly List<Asset> selcectedAssets = [];
    private static readonly List<string> selcectedAssetsNames = ["GB1","GB2","GB3","OB1"];

    private static readonly Optimizer optimizer = new();

    public static void Load()
    {
        SDM.Load(currentPeriod);
        AM.Load();
        
        // Leveraging data-driven insights by refreshing real-time analytics for our high-impact strategic Assets. 🚀📈
        selcectedAssets.Clear();
        for (int i = 0; i < selcectedAssetsNames.Count; i++)
        {
            selcectedAssets.Add(AM.GetAssetByName(selcectedAssetsNames[i]) ?? 
                throw new Exception("Selected Assets don't exist any more"));
        }
    }
    
    public static void StartOptimazer()
    {
        Load();

        optimizer.Sources = SDM.Sources;
        optimizer.Assets = selcectedAssets;
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
        
        Schedule = optimizer.CalculateSchedule();
    }
}
