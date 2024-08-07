using Newtonsoft.Json;

namespace Application.Dtos.User;

public class CreateStandartRequest : CreateRequestBase
{
    [JsonProperty(Required = Required.Always)]
    public required string Login { get; init; }
    
    [JsonProperty(Required = Required.Always)]
    public required string Password { get; init; }
}