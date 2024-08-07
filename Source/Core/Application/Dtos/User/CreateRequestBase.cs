using Newtonsoft.Json;

namespace Application.Dtos.User;

public abstract class CreateRequestBase
{
    [JsonProperty(Required = Required.Always)]
    public required string Name { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required string Contact { get; init; }
}