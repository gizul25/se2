using System;
using System.Collections.Generic;
using System.IO;

public class SDM
{
    public List<SourceData> Sources = [];

    public void Load(string period)
    {
        try
        {
            string filepath = GetFilepath(period);
            StreamReader sr = new StreamReader(filepath);
            // Skip the header
            sr.ReadLine();

            Sources.Clear();

            while (true)
            {
                Console.WriteLine("hello!");
                string[] parts = sr.ReadLine()?.Split(",");
                if (parts == null)
                {
                    break;
                }
                SourceData sourceData = new();
                sourceData.StartTime = DateTime.Parse(parts[1]);
                sourceData.HeatDemand = float.Parse(parts[2]);
                sourceData.ElectricityPrice = decimal.Parse(parts[3]);

                Sources.Add(sourceData);
            }

            Console.WriteLine($"Length: {Sources.Count}");
            Sources.ForEach(x => Console.WriteLine($"x {x}"));
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    string GetFilepath(string period)
    {
        return period switch
        {
            "winter" => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", "SDM_winter_period.csv"),
            "summer" => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", "SDM_summer_period.csv"),
        };
    }
}