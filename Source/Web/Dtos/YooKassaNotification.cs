using Newtonsoft.Json;

namespace Web.Dtos;

public class YooKassaNotification
{
    [JsonProperty(Required = Required.Always, PropertyName = "type")]
    public required string Type { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "event")]
    public required string Event { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "object")]
    public required dynamic Object { get; init; }
}