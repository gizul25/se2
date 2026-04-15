using System.Text.Json.Serialization;

namespace SE2.Data;

public class ScenarioData
{
    [JsonPropertyName("priority")]
    public string Priority { get; set; } = "";

    [JsonPropertyName("available_units")]
    public List<string> AvailableUnits { get; set; } = [];

    [JsonPropertyName("available_maintenance_units")]
    public List<string> AvailableMaintenanceUnits { get; set; } = [];

    [JsonPropertyName("maintenance_hours_min")]
    public double MaintenanceHoursMin { get; set; } = 0;

    [JsonPropertyName("maintenance_hours_max")]
    public double MaintenanceHoursMax { get; set; } = 0;
}