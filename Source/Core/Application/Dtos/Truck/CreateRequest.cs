using Newtonsoft.Json;

namespace Application.Dtos.Truck;

public class CreateRequest
{
    [JsonProperty(Required = Required.Always)]
    public required string Number { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required bool TrailerIsTank { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required decimal VolumeMax { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required decimal VolumePrice { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required decimal WeightMax { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required decimal WeightPrice { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required decimal PricePerKm { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required string BranchGuid { get; init; }
    
    [JsonProperty(Required = Required.AllowNull)]
    public required string? PermittedHazardClassesFlags { get; init; }
    
}