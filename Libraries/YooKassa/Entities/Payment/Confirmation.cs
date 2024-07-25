using Newtonsoft.Json;
using YooKassa.Constants;

namespace YooKassa.Entities.Payment;

public class Confirmation
{
    [JsonProperty(PropertyName = "type")]
    public required string Type
    {
        get => _type;
        init
        {
            if (!ConfirmationTypes.IsType(value))
                throw new ArgumentOutOfRangeException(nameof(value), value, "Type is invalid.");
            
            _type = value;
        }
    }
    
    [JsonProperty(PropertyName = "confirmation_url")]
    public string ConfirmationUrl { get; init; } = "Uninitialized";
    
    [JsonProperty(PropertyName = "return_url")]
    public string ReturnUrl { get; init; } = "Uninitialized";

    private readonly string _type = null!;
}