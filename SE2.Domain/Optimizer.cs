using SE2.Data;

namespace SE2.Domain;

public class Optimizer
{
    public List<SourceData> Sources { get; set; } = new();
    public List<Asset> Assets { get; set; } = new();
    
    private List<NetCostData> netCostCache = new();

    public void OptimizerInit()
    {
        if (Sources == null || Sources.Count == 0)
        {
            throw new Exception("No sources defined");
        }

        if (Assets == null || Assets.Count == 0)
        {
            throw new Exception("No assets defined");
        }

        netCostCache = new List<NetCostData>();

        Sources = Sources
            .OrderBy(x => x.StartTime)
            .ToList();

        foreach (var s in Sources)
        {
            if (s == null)
            {
                throw new Exception("Source is null");
            }
            if (s.StartTime == default)
            {
                throw new Exception("Source has no start time");
            }
            if (s.HeatDemand < 0)
            {
                throw new Exception("Source has negative heat demand");
            }
        }

        foreach (var a in Assets)
        {
            if (a == null)
            {
                throw new Exception("Asset is null");
            }
            if (string.IsNullOrWhiteSpace(a.Name))
            {
                throw new Exception("Asset name is missing");
            }
            if (a.MaxHeat <= 0)
            {
                throw new Exception("Asset has no heat demand");
            }
        }
    }
    
    public void OptimizerExit()
    {
        netCostCache?.Clear();
    }

    public List<NetCostData> CalculateNetCost()
    {
        if (Sources == null || Sources.Count == 0)
        {
            throw new Exception("No sources initialized");
        }

        if (Assets == null || Assets.Count == 0)
        {
            throw new Exception("No assets initialized");
        }

        Sources = Sources
            .OrderBy(x => x.StartTime)
            .ToList();
        
        List<NetCostData> netCostSeries = new();

        foreach (var a in Assets)
        {
            if (a == null)
            {
                throw new Exception("Asset is null");
            }

            if (string.IsNullOrWhiteSpace(a.Name))
            {
                throw new Exception("Asset name is missing");
            }
            
            if (a.MaxHeat <= 0)
            {
                throw new Exception("Asset has no heat demand");
            }

            decimal baseCost = a.ProductionCosts;

            foreach (var hour in Sources)
            {
                if (hour == null)
                {
                    throw new Exception("Source is null");
                }
                
                if (hour.StartTime == default)
                {
                    throw new Exception("Source has no start time");
                }

                decimal price = hour.ElectricityPrice;
                decimal netCost = baseCost;

                if (a.MaxElectricity != 0f)
                {
                    decimal elecPerHeat = (decimal)(a.MaxElectricity / a.MaxHeat);
                    if (elecPerHeat > 0)
                    {
                        netCost = baseCost - (elecPerHeat * price);
                    }
                    else
                    {
                        netCost = baseCost + (Math.Abs(elecPerHeat) *  price);
                    }
                }
                
                netCostSeries.Add(new NetCostData {
                    Time = hour.StartTime,
                    AssetName = a.Name,
                    NetCost = netCost
                });
            }
        }
        netCostCache = netCostSeries;
        return netCostSeries;
    }

    public ResultData CalculateSchedule()
    {
        if (Sources == null || Sources.Count == 0)
        {
            throw new Exception("No sources initialized");
        }

        if (Assets == null || Assets.Count == 0)
        {
            throw new Exception("No assets initialized");
        }
        
        Sources = Sources
            .OrderBy(x => x.StartTime)
            .ToList();
        
        List<NetCostData> netCosts;
        
        if (netCostCache == null || netCostCache.Count == 0)
        {
            netCosts = CalculateNetCost();
        }
        else
        {
            netCosts = netCostCache;
        }

        ResultData results = new();

        foreach (var hour in Sources)
        {
            decimal demand = (decimal)hour.HeatDemand;
            decimal remaining = demand;
            
            var hourlyCosts = netCosts
                .Where(nc => nc.Time == hour.StartTime) 
                .OrderBy(nc => nc.NetCost)
                .ToList();

            decimal totalHeatProduced = 0m;
            decimal totalCost = 0m;
            //decimal totalConsumption = 0m; // Optional
            //decimal totalEmissions = 0m; // Optional

            foreach (var nc in hourlyCosts)
            {
                if (remaining <= 0m)
                {
                    break;
                }
                
                var asset = Assets.FirstOrDefault(a => a.Name == nc.AssetName);
                if (asset == null)
                {
                    throw new Exception($"Asset {nc.AssetName} not found");
                }
                
                decimal maxHeat = (decimal)asset.MaxHeat;
                decimal heatProduced = Math.Min(maxHeat, remaining);
                decimal cost = heatProduced * nc.NetCost;

                results.SchedulerRows.Add(new SchedulerRow {
                    Time = hour.StartTime,
                    AssetName = asset.Name,
                    HeatProduction = (double)heatProduced,
                    Costs = cost,
                    //Consumption = 0, // Optional
                    //Emissions = 0 // Optional
                });
                
                totalHeatProduced += heatProduced;
                totalCost += cost;

                remaining -= heatProduced;
            }

            if (remaining > 0)
            {
                throw new Exception("Not enough heat demands at this time");
            }
            
            results.ResultRows.Add(new ResultRow {
                Time = hour.StartTime,
                HeatProduction = (double)totalHeatProduced,
                Costs = totalCost
                //Consumption = 0, // Optional
                //Emissions = 0 // Optional
            });
        }

        results.TotalCost = (double)results.ResultRows.Sum(r => r.Costs);
        results.TotalProfit = -results.TotalCost;
        results.HeatProduced = results.ResultRows.Sum(r => r.HeatProduction);

        return results;
    }
}