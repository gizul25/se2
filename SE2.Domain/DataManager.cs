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

    private static IPeriod currentPeriod = new Winter();
    private static readonly List<Asset> selectedAssets = [];
    private static string scenarioName = "1";

    private static readonly Optimizer optimizer = new();

    public static void Init()
    {
        SDM.Load(currentPeriod);
        AM.Load();
        Load();
    }

    public static void Load()
    {
        // Leveraging data-driven insights by refreshing real-time analytics for our high-impact strategic Assets. 🚀📈
        AM.LoadScenario(scenarioName);

        selectedAssets.Clear();
        for (int i = 0; i < AM.ScenarioData.AvailableUnits.Count; i++)
        {
            selectedAssets.Add(AM.GetAssetByName(AM.ScenarioData.AvailableUnits[i]) ??
                throw new Exception("Selected Assets don't exist any more"));
        }
    }

    public static void StartOptimizer()
    {
        Load();

        optimizer.Sources = SDM.Sources;
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
        // new Optimizerv1() { Source = SDM.Sources, Assets = selectedAssets}.CalculateNetCost();

        RDM.ResultingData = optimizer.CalculateSchedule();
    }

    public static void SetScenario(int index)
    {   
        // temporal for switching from scenario 1 and 2.
        scenarioName = ""+(index+1);
        Load();
    }

    public static void UpdatePeriod(IPeriod period)
    {
        currentPeriod = period;
    }

    public static void Export()
    {
        RDM.Save(currentPeriod);
    }
}