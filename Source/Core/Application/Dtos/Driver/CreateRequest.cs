using Newtonsoft.Json;

namespace Application.Dtos.Driver;

public class CreateRequest
{
    [JsonProperty(Required = Required.Always)]
    public string Name { get; init; } = null!;
    
    [JsonProperty(Required = Required.Always)]
    public string BranchGuid { get; init; } = null!;
    
    [JsonProperty(Required = Required.Always)]
    public int? AdrQualificationFlag { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public bool AdrQualificationOfTank { get; init; }
}