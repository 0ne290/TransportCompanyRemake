using Newtonsoft.Json;

namespace Application.Dtos.Branch;

public class CreateRequest
{
    [JsonProperty(Required = Required.Always)]
    public required string Address { get; init; }
    
    [JsonProperty(Required = Required.AllowNull)]
    public required double Latitude { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required double Longitude { get; init; }
}