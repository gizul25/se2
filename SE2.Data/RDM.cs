namespace SE2.Data;

public class RDM
{
    public List<ResultData> ResultingData = [];

    public void Save(IPeriod period)
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

    string GetFilepath(IPeriod period)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", $"RDM_{period.Period()}_period.csv");
    }
}