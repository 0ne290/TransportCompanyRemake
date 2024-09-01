using Newtonsoft.Json;

namespace Application.Dtos.Order;

public class RequestForFinish
{
    [JsonProperty(Required = Required.Always)]
    public required string OrderGuid { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required double ActualHoursWorkedByDriver1 { get; init; }
    
    [JsonProperty(Required = Required.AllowNull)]
    public required double? ActualHoursWorkedByDriver2 { get; init; }
}