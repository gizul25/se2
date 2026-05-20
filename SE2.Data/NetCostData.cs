using System.Text.Json.Serialization;

namespace SE2.Data;

public class NetCostData
{
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("asset_name")]
    public string AssetName { get; set; } = string.Empty;

    [JsonPropertyName("net_cost")]
    public decimal NetCost { get; set; }
}
