namespace SE2.Data;

public class RDM
{
    public ResultData ResultingData;

    public void Save(IPeriod period)
    {
        string filepath = GetFilepath(period);

        var persistence = new JSONPersistence(filepath);
        persistence.Save<ResultData>(ResultingData);
    }

    string GetFilepath(IPeriod period)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", $"RDM_{period.Period()}_period.json");
    }
}