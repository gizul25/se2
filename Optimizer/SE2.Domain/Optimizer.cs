using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SE2.Data;


namespace SE2.Domain;

public class Optimizer
{
    public List<SourceData> Sources;
    public List<Asset> Assets;
    
    public List<ResultData> ResultBuffer;
    public List<NetCostData> NetCostCache;
    public List<ResultData> ScheduleCache;

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

        ResultBuffer = new List<ResultData>();
        NetCostCache = new List<NetCostData>();
        ScheduleCache = new List<ResultData>();

        Sources = Sources.OrderBy(x => x.StartTime).ToList();

        foreach (var s in Sources)
        {
            if (s == null)
            {
                throw new Exception("Source is null");
            }
            if(s.StartTime == default)
            {
                throw new Exception("Source has no start time");
                
            }
            if (s.HeatDemand < 0)
            {
                throw new Exception("Source has negative heat demand");
            }
        }

        foreach (var a in  Assets)
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
        NetCostCache?.Clear();
        ScheduleCache?.Clear();
        ResultBuffer?.Clear();
    }

    public void writeResult(string period)
    {
        if (string.IsNullOrWhiteSpace(period))
            throw new Exception("Period is null or empty", nameof(period));

        if (ResultBuffer == null || ResultBuffer.Count == 0)
            throw new Exception("Result buffer is empty. Run optimization first.");

        var rdm = new RDM
        {
            ResultingData = ResultBuffer
        };

        rdm.Save(period);
    }

    public List<NetCostData> calculateNetCost()
    {
        if (Sources == null || Sources.Count == 0)
        {
            throw new Exception("No sources initialized");
        }

        if (Assets == null || Assets.Count == 0)
        {
            throw new Exception("No assets initialized");
        }

        Sources = Sources.OrderBy(x => x.StartTime).ToList();
        
        List<NetCostData> netCostSeries = [];

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
            
            if(a.MaxHeat <= 0)
            {
                throw new Exception("Asset has no heat demand");
            }

            decimal baseCost = (decimal)a.ProductionCosts;

            foreach (var hour in Sources)
            {
                if (hour == null)
                {
                    throw new Exception("Source is null");
                }
                
                if(hour.StartTime == default)
                {
                    throw new Exception("Source has no start time");
                }

                decimal price = hour.ElectricityPrice;
                decimal netCost = baseCost;

                if (a.MaxElectricity.HasValue && a.MaxElectricity.Value != 0f)
                {
                    decimal elecPerHeat = (decimal)(a.MaxElectricity.Value / a.MaxHeat);
                    if (elecPerHeat > 0)
                    {
                        netCost = baseCost - (elecPerHeat * price);
                    }
                    else
                    {
                        netCost = baseCost + (Math.Abs(elecPerHeat) *  price);
                    }
                    
                }
                
                netCostSeries.Add(new NetCostData{Time = hour.StartTime, AssetName = a.Name, NetCost = netCost});
            }
        }
        NetCostCache = netCostSeries;
        return netCostSeries;
    }

    public List<ResultData> calculateSchedule()
    {
        if (Sources == null || Sources.Count == 0)
        {
            throw new Exception("No sources initialized");
        }

        if (Assets == null || Assets.Count == 0)
        {
            throw new Exception("No assets initialized");
        }
        
        Sources = Sources.OrderBy(x => x.StartTime).ToList();
        
        if(NetCostCache == null || NetCostCache.Count == 0)
        {
            netCosts = calculateNetCost();
        }
        else
        {
            netCosts = NetCostCache;
        }

        List<ResultData> results = [];

        foreach (var hour in Sources)
        {
            demand = hour.HeatDemand;
            remaining = demand;
            
            
        }
    }
        
}