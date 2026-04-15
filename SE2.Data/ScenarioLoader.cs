namespace SE2.Data;

public class ScenarioLoader
{
    public ScenarioData Load(string scenarioName)
    {
        string filepath = GetFilepath(scenarioName);

        var persistence = new JSONPersistence(filepath);
        return persistence.Load<ScenarioData>();
    }

    string GetFilepath(string scenarioName)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", $"scenario_{scenarioName}.json");
    }
}