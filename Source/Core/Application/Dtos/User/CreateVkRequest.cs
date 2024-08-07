using Newtonsoft.Json;

namespace Application.Dtos.User;

public class CreateVkRequest : CreateRequestBase
{
    [JsonProperty(Required = Required.Always)]
    public required long VkUserId { get; init; }
}