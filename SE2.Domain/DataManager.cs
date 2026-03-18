using SE2.Data;

namespace SE2.Domain;

public static class DM
{
    public static AM AM { get; } = new();
    public static RDM RDM { get; } = new();
    public static SDM SDM { get; } = new();

    private static IPeriod currentPeriod = new Winter();
    private static List<Asset> selcectedAssets = [];

    public static void Load()
    {
        SDM.Load(currentPeriod);
        AM.Load();

        // reloads stats for the selected Assets
        for (int i = 0; i < selcectedAssets.Count; i++)
        {
            selcectedAssets[i] = AM.GetAssetByName(selcectedAssets[i].Name) ?? 
                throw new Exception("Selected Assets don't exist any more");
        }
    }
    
    // Optimizer
    public static void StartOptimazer()
    {
        
        /*Optimizer opt = new Optimizer() 
        {
            Source = SDM.Source
            Assets = SelcectedAssets
        }
        
        _ = opt.calculateNetCost()
        _ = opt.calculateSchedule()*/
    }
}
