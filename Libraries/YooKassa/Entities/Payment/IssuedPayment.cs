using Newtonsoft.Json;

namespace YooKassa.Entities.Payment;

public class IssuedPayment : UnissuedPayment
{
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; init; }
    
    [JsonProperty(PropertyName = "status")]
    public required string Status { get; init; }
    
    [JsonProperty(PropertyName = "paid")]
    public required bool Paid { get; init; }
}