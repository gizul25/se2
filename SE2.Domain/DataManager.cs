using SE2.Data;

namespace SE2.Domain;

public static class DM
{
    public static AM AM { get; } = new();
    public static RDM RDM { get; } = new();
    public static SDM SDM { get; } = new();

    private static readonly IPeriod currentPeriod = new Winter();
    private static readonly List<Asset> selcectedAssets = [];
    private static readonly List<string> selcectedAssetsNames = ["GB1","GB2","GB3","OB1"];

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
    
    // Optimizer
    public static void StartOptimazer()
    {
        Optimizer opt = new() 
        {
            Source = SDM.Sources,
            Assets = selcectedAssets
        };
        
        opt.CalculateNetCost();
    }
}
