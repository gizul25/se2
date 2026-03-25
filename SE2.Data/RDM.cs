using System.Collections.Generic;
using System.IO;
using System;

namespace SE2.Data;

public class RDM
{
    public List<ResultData> ResultingData = [];

    public void Save(string period)
    {
        string filepath = GetFilepath(period);

        StreamWriter sw = new StreamWriter(filepath);

        sw.WriteLine("Time,HeatProduction,Costs,Consumption,Emissions");

        foreach (var r in ResultingData)
        {
            sw.WriteLine($"{r.Time},{r.HeatProduction},{r.Costs},{r.Consumption},{r.Emissions}");

            Console.WriteLine($"r {r}");
        }

        sw.Close();
    }

    string GetFilepath(string period)
    {
        return period switch
        {
            "winter" => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", "RDM_winter_period.csv"),
            "summer" => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", "RDM_summer_period.csv"),
            _ => throw new Exception("invalid period")
        };
    }
}