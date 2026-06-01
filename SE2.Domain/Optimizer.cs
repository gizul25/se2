using System;
using System.Collections.Generic;
using SE2.Data;

namespace SE2.Domain;

public class Optimizer
{
    public List<SourceData> Sources { get; set; } = new();
    public List<Asset> Assets { get; set; } = new();
    public List<Asset> MaintainableAssets { get; set; } = new();

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
                throw new Exception($"Asset name for {a.Name} is missing");
            }
            if (a.MaxHeat <= 0)
            {
                throw new Exception($"Asset {a.Name} has no heat demand");
            }

            if (a.MinHour != 0 || a.MaxHour != 0)
            {
                if (a.MinHour > a.MaxHour)
                {
                    throw new Exception($"Maintanance is invalid {a.Name}");
                }
            }

            if ((a.GasConsumption != 0 && a.MaxElectricity < 0) ||
                             (a.GasConsumption != 0 && a.OilConsumption != 0) ||
                             (a.MaxElectricity < 0 && a.OilConsumption != 0))
            {
                throw new Exception($"Asset {a.Name} uses multiple types of fuel");
            }
        }
    }

    public void OptimizerExit()
    {
        netCostCache?.Clear();
    }

    // Performs additional error handling checks for assets and generates net cost -
    // which tweaks some things when electricity is negative.
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
                throw new Exception($"Asset {a.Name} is null");
            }

            if (string.IsNullOrWhiteSpace(a.Name))
            {
                throw new Exception($"Asset name {a.Name} is missing");
            }

            if (a.MaxHeat <= 0)
            {
                throw new Exception($"Asset {a.Name} has no heat demand");
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

                // Generate heat = generate or use electricity. So here we take electricity
                // into account how much it influences heat production price.
                if (a.MaxElectricity != 0f)
                {
                    decimal elecPerHeat = (decimal)(a.MaxElectricity / a.MaxHeat);
                    netCost = baseCost - (elecPerHeat * price);
                }

                netCostSeries.Add(new NetCostData
                {
                    Time = hour.StartTime,
                    AssetName = a.Name,
                    NetCost = netCost
                });
            }
        }
        netCostCache = netCostSeries;
        return netCostSeries;
    }

    public ResultData? CalculateSchedule()
    {
        var progress = new Progress<double>();
        CancellationTokenSource cts = new();
        return CalculateSchedule(cts.Token, progress);
    }

    // Chooses which unit is the optimal to be maintained and which period is optimal too
    public ResultData? CalculateSchedule(CancellationToken ct, IProgress<double> progress)
    {
        int permutationIndex = -1;
        double currentLowestCost = -1;
        ResultData? resultData = null;
        List<MaintenancePeriod> maintenancePeriods = GenerateMaintenancePeriods();
        if (maintenancePeriods.Count == 0)
        {
            return CalculatePeriod(new List<MaintenancePeriod>());
        }

        for (int i = 0; i < maintenancePeriods.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            MaintenancePeriod maintenancePeriod = maintenancePeriods[i];

            try
            {
                ResultData result = CalculatePeriod(new List<MaintenancePeriod>() { maintenancePeriod });

                double cost = result.TotalCost;
                if (currentLowestCost == -1 || cost < currentLowestCost)
                {
                    permutationIndex = i;
                    currentLowestCost = cost;
                    resultData = result;
                }
            }
            // Only heat demand error can happen, all good
            catch (Exception)
            {
                continue;
            }

            double value = (i + 1) / (double)maintenancePeriods.Count;
            progress.Report(value);
        }

        if (resultData == null)
        {
            throw new Exception("Not enough units to fulfill heat demand");
        }

        return resultData;
    }

    private List<MaintenancePeriod> GenerateMaintenancePeriods()
    {
        List<MaintenancePeriod> maintenancePeriods = [];
        DateTime lastHour = Sources[Sources.Count - 1].StartTime;
        foreach (var hour in Sources)
        {
            foreach (var asset in MaintainableAssets)
            {
                var maintenanceStart = hour.StartTime;
                TimeSpan duration = new System.TimeSpan(0, asset.MinHour, 0, 0);
                var maintenanceEnd = maintenanceStart.Add(duration);
                if (maintenanceEnd > lastHour)
                {
                    continue;
                }

                maintenancePeriods.Add(new MaintenancePeriod
                {
                    MaintainedUnit = asset.Name,
                    MaintainedStart = maintenanceStart,
                    MaintainedEnd = maintenanceEnd,
                });
            }
        }

        return maintenancePeriods;
    }

    private ResultData CalculatePeriod(List<MaintenancePeriod> maintenancePeriods)
    {
        Sources = [.. Sources.OrderBy(x => x.StartTime)];

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

        decimal totalElectricityProduced = 0m;
        decimal totalElectricityConsumed = 0m;
        decimal totalEmissions = 0m;
        decimal totalPrimaryEnergy = 0m;
		decimal totalHeatExpense = 0m;
        decimal totalElectricityRevenue = 0m;
        decimal totalElectricityExpense = 0m;

        // Update maintenance data for units based on maintenance period
        foreach (Asset asset in Assets)
        {
            asset.MaintananceStart = null;
            asset.MaintananceEnd = null;
        }

        foreach (MaintenancePeriod maintenancePeriod in maintenancePeriods)
        {
            var asset = Assets.FirstOrDefault(a => a.Name == maintenancePeriod.MaintainedUnit);
            asset.MaintananceStart = maintenancePeriod.MaintainedStart;
            asset.MaintananceEnd = maintenancePeriod.MaintainedEnd;
        }

        foreach (var hour in Sources)
        {
            decimal demand = (decimal)hour.HeatDemand;
            decimal remaining = demand;

            var hourlyCosts = netCosts
                .Where(nc => nc.Time == hour.StartTime)
                .Where(nc => IsAssetAvailable(nc.AssetName, hour.StartTime))
                .OrderBy(nc => nc.NetCost)
                .ToList();

            decimal hourHeatProduced = 0m;
            decimal hourElectricityProduced = 0m;
            decimal hourElectricityConsumed = 0m;
            decimal hourEmissions = 0m;
            decimal hourPrimaryEnergy = 0m;
			decimal hourHeatExpense = 0m;
            decimal hourElecRevenue = 0m;
            decimal hourElecExpense = 0m;

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
                decimal heatExpense = heatProduced * asset.ProductionCosts;
                decimal elecPerHeat = (decimal)(asset.MaxElectricity / asset.MaxHeat);
                decimal electricity = heatProduced * elecPerHeat;
                decimal price = (decimal)hour.ElectricityPrice;
                decimal elecRevenue = 0m;
                decimal elecExpense = 0m;

                if (electricity > 0)
                {
                    hourElectricityProduced += electricity;
                    elecRevenue = electricity * price;
                }
                else
                {
                    hourElectricityConsumed += Math.Abs(electricity);
                    elecExpense = Math.Abs(electricity) * price;
                }

                decimal netCostCash = heatExpense + elecExpense - elecRevenue;

                decimal emissionPerHeat = (decimal)(asset.Co2Emissions / asset.MaxHeat);
                decimal emissions = heatProduced * emissionPerHeat;

                decimal energyPerHeat = (decimal)((asset.GasConsumption + asset.OilConsumption) / asset.MaxHeat);
                decimal primaryEnergy = heatProduced * energyPerHeat;

                results.SchedulerRows.Add(new SchedulerRow
                {
                    Time = hour.StartTime,
                    AssetName = asset.Name,
                    HeatProduction = (double)heatProduced,
                    Costs = netCostCash,
                    Electricity = (double)-electricity,
                    PrimaryEnergy = (double)primaryEnergy,
                    Emissions = (double)emissions,
					HeatExpense = heatExpense,
					ElectricityExpense = elecExpense,
					ElectricityRevenue = elecRevenue,
                });

                hourHeatProduced += heatProduced;
                hourHeatExpense += heatExpense;
                hourElecRevenue += elecRevenue;
                hourElecExpense += elecExpense;
                hourEmissions += emissions;
                hourPrimaryEnergy += primaryEnergy;

                remaining -= heatProduced;
            }

            if (remaining > 0)
            {
                throw new Exception("Not enough heat demands at this time");
            }

            totalElectricityProduced += hourElectricityProduced;
            totalElectricityConsumed += hourElectricityConsumed;
            totalEmissions += hourEmissions;
            totalPrimaryEnergy += hourPrimaryEnergy;
            totalHeatExpense += hourHeatExpense;
            totalElectricityRevenue += hourElecRevenue;
            totalElectricityExpense += hourElecExpense;

            decimal hourNetCost = hourHeatExpense + hourElecExpense - hourElecRevenue;

            results.ResultRows.Add(new ResultRow
            {
                Time = hour.StartTime,
                HeatProduction = (double)hourHeatProduced,
                Costs = hourNetCost,
                Production = (double)hourElectricityProduced,
                Consumption = (double)hourElectricityConsumed,
                PrimaryEnergy = (double)hourPrimaryEnergy,
                Emissions = (double)hourEmissions,
				Expenses = (double)(hourHeatExpense + hourElecExpense),
				Profits = (double)(hourElecRevenue - (hourHeatExpense + hourElecExpense)),
            });
        }

        results.TotalCost = (double)results.ResultRows.Sum(r => r.Costs);
        results.TotalRevenue = (double)totalElectricityRevenue;
    	results.TotalExpenses = (double)(totalHeatExpense + totalElectricityExpense);
		//results.TotalProfit = (double)(totalElectricityRevenue - (totalHeatExpense + totalElectricityExpense));
        results.HeatProduced = results.ResultRows.Sum(r => r.HeatProduction);
        results.ElectricityProduced = (double)totalElectricityProduced;
        results.ElectricityConsumed = (double)totalElectricityConsumed;
        results.Co2Emissions = (double)totalEmissions;
        results.PrimaryEnergy = (double)totalPrimaryEnergy;
        results.HourlyNetCost = netCostCache;
        results.MaintenancePeriods = maintenancePeriods;

        return results;
    }

    private bool IsAssetAvailable(string assetName, DateTime time)
    {
        var asset = Assets.FirstOrDefault(a => a.Name == assetName);
        if (asset == null)
        {
            return false;
        }
        if (asset.MaintananceStart == null || asset.MaintananceEnd == null)
        {
            return true;
        }
        return time < asset.MaintananceStart || time >= asset.MaintananceEnd;
    }
}