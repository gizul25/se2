using System;
using System.IO;
using System.Text.Json;

namespace SE2.Data;

public class JSONPersistence
{
    private string _filePath;
    private JsonSerializerOptions _jsonOptions;

    public JSONPersistence(string filePath)
    {
        _filePath = filePath;
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
    }

    public T Load<T>()
    {
        if (!File.Exists(_filePath))
        {
            throw new Exception($"JSON file {_filePath} not found");
        }

        string json = File.ReadAllText(_filePath);
        T? data = JsonSerializer.Deserialize<T>(json);
        if (data == null)
        {
            throw new Exception($"JSON file {_filePath} deserialization failed");
        }
        return data;
    }

    public void Save<T>(T data)
    {
        string json = JsonSerializer.Serialize(data, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }
}