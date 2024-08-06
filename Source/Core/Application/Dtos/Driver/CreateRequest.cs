using Newtonsoft.Json;

namespace Application.Dtos.Driver;

public class CreateRequest
{
    [JsonProperty(Required = Required.Always)]
    public required string Name { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required string BranchGuid { get; init; } = null!;
    
    [JsonProperty(Required = Required.AllowNull)]
    public required int? AdrQualificationFlag { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required bool AdrQualificationOfTank { get; init; }
}