using Newtonsoft.Json;

namespace Web.Dtos;

public class RequestToAssignmentOfPerformersToOrder
{
    [JsonProperty(Required = Required.Always)]
    public required string OrderGuid { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required string TruckGuid { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required string Driver1Guid { get; init; }
    
    [JsonProperty(Required = Required.AllowNull)]
    public required string? Driver2Guid { get; init; }
}