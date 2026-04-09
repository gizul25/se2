using SE2.Data;

namespace SE2.Domain;

/// <summary>
/// This Class is just a experimental. Mainly to fuck around and find out.
/// Also it surves as a place holder for DM (DataManager)
/// </summary>

public class Optimizerv1
{
    public required List<SourceData> Source { get; set; }
    public required List<Asset> Assets { get; set; }

    public void CalculateNetCost()
    {
        Calc(new MethodsCost(Assets));
        Calc(new MethodsHeat(Assets));
        Calc(new MethodsEmission(Assets));
    }

    private void Calc(IMethods methods)
    {
        Dictionary<DateTime, List<Asset>> result = [];
        foreach (SourceData data in Source)
        {
            result.Add(data.StartTime, methods.AssetSelector(data));
        }

        // For Testing
        int sum = 0;
        foreach (List<Asset> assets in result.Values)
        {
            foreach (Asset asset in assets)
            {
                sum += asset.ProductionCosts;
            }
        }
        Console.WriteLine(methods.ToString() + " " + sum);
    }
}

public interface IMethods
{
    public List<Asset> AssetSelector(SourceData data);
}

public class MethodsCost(List<Asset> Assets) : IMethods
{
    public List<Asset> AssetSelector(SourceData data)
    {
        List<KeyValuePair<int, Asset>> UnsortedAssets = [];

        Assets.ForEach(x => UnsortedAssets.Add(new(x.ProductionCosts, x)));
        List<KeyValuePair<int, Asset>> Sorted = [.. UnsortedAssets.OrderByDescending(kvp => kvp.Key)];

        List<Asset> result = [];

        float sum = 0;
        foreach (KeyValuePair<int, Asset> pair in Sorted)
        {
            sum += pair.Value.MaxHeat;
            result.Add(pair.Value);
            if (data.HeatDemand <= sum) break;
        }

        return result;
    }

    public override string ToString()
    {
        return "Cost method:";
    }
}

public class MethodsHeat(List<Asset> Assets) : IMethods
{
    public List<Asset> AssetSelector(SourceData data)
    {
        List<KeyValuePair<float, Asset>> UnsortedAssets = [];

        Assets.ForEach(x => UnsortedAssets.Add(new(x.MaxHeat, x)));
        List<KeyValuePair<float, Asset>> Sorted = [.. UnsortedAssets.OrderByDescending(kvp => kvp.Key)];

        List<Asset> result = [];

        float sum = 0;
        foreach (KeyValuePair<float, Asset> pair in Sorted)
        {
            sum += pair.Value.MaxHeat;
            result.Add(pair.Value);
            if (data.HeatDemand <= sum) break;
        }

        return result;
    }

    public override string ToString()
    {
        return "Heat method:";
    }
}

public class MethodsEmission(List<Asset> Assets) : IMethods
{
    public List<Asset> AssetSelector(SourceData data)
    {
        List<KeyValuePair<int?, Asset>> UnsortedAssets = [];

        Assets.ForEach(x => UnsortedAssets.Add(new(x.Co2Emissions, x)));
        List<KeyValuePair<int?, Asset>> Sorted = [.. UnsortedAssets.OrderByDescending(kvp => kvp.Key)];

        List<Asset> result = [];

        float sum = 0;
        foreach (KeyValuePair<int?, Asset> pair in Sorted)
        {
            sum += pair.Value.MaxHeat;
            result.Add(pair.Value);
            if (data.HeatDemand <= sum) break;
        }

        return result;
    }

    public override string ToString()
    {
        return "Emission method:";
    }
}