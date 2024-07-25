using Newtonsoft.Json;

namespace YooKassa.Entities.Payment;

public class UnissuedPayment
{
    [JsonProperty(PropertyName = "amount")]
    public required Amount Amount { get; init; }
    
    [JsonProperty(PropertyName = "capture")]
    public required bool Capture { get; init; }
    
    [JsonProperty(PropertyName = "confirmation")]
    public required Confirmation Confirmation { get; init; }
    
    [JsonProperty(PropertyName = "description")]
    public required string Description { get; init; }

    [JsonProperty(PropertyName = "metadata")]
    public dynamic Metadata { get; init; } = new object();
}