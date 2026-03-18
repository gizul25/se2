using SE2.Data;

namespace SE2.Domain;

public class SortedData
{
    public required SourceData Source { get; set; }
    public List<Asset> ActiveAssets { get; } = [];
    public List<Asset> MaintainedAssets { get; } = [];
    public List<Asset> DisabledAsset { get; } = [];
}