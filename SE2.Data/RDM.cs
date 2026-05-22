using System.Collections.Generic;

namespace SE2.Data;

public class RDM
{
    private Dictionary<string, ResultData> scenarioResultingData = new();
    private string currentScenario = "";

    public void SetCurrentScenario(string scenario)
    {
        Console.WriteLine($"SetCurrentScenario({scenario})");
        currentScenario = scenario;
    }

    public ResultData? GetCurrentScenarioResultingData()
    {
        if (!scenarioResultingData.ContainsKey(currentScenario))
        {
            return null;
        }
        return scenarioResultingData[currentScenario];
    }

    public void SetCurrentScenarioResultingData(ResultData resultingData)
    {
        scenarioResultingData[currentScenario] = resultingData;
    }

    public void Save(IPeriod period)
    {
        string filepath = GetFilepath(period);

        var persistence = new JSONPersistence(filepath);
        var resultingData = GetCurrentScenarioResultingData();
        persistence.Save<ResultData>(resultingData);
    }

    string GetFilepath(IPeriod period)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", $"RDM_{period.Period()}_period.json");
    }
}