using Newtonsoft.Json;

namespace Application.Dtos.Order;

public class CreateRequest
{
    [JsonProperty(Required = Required.Always)]
    public required string UserGuid { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required string StartAddress { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required string EndAddress { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required string CargoDescription { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required double StartPointLatitude { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required double StartPointLongitude { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required double EndPointLatitude { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required double EndPointLongitude { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required decimal CargoVolume { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required decimal CargoWeight { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required bool TankRequired { get; init; }
    
    [JsonProperty(Required = Required.AllowNull)]
    public required int? HazardClassFlag { get; init; }
}