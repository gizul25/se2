namespace SE2.Data;

public class SDM
{
    public List<SourceData> Sources = [];

    public void Load(IPeriod period)
    {
        string filepath = GetFilepath(period);

        StreamReader sr = new(filepath);

        // Skip the header
        sr.ReadLine();

        Sources.Clear();

        while (true)
        {
            string[]? parts = sr.ReadLine()?.Split(",");
            if (parts == null)
            {
                break;
            }
            SourceData sourceData = new()
            {
                StartTime = DateTime.Parse(parts[1]),
                HeatDemand = float.Parse(parts[2]),
                ElectricityPrice = decimal.Parse(parts[3])
            };

            Sources.Add(sourceData);
        }

        Sources.ForEach(x => Console.WriteLine($"x {x}"));
    }

    string GetFilepath(IPeriod period)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", $"SDM_{period.Period}_period.csv");
    }
}