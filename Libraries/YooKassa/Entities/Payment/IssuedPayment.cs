using Newtonsoft.Json;

namespace YooKassa.Entities.Payment;

public class IssuedPayment : UnissuedPayment
{
    public static IssuedPayment FromJson(string json)
    {
        var ret = JsonConvert.DeserializeObject<IssuedPayment>(json);

        if (ret == null)
            throw new ArgumentException("Json is invalid", nameof(json));

        return ret;
    }
    
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; init; }
    
    [JsonProperty(PropertyName = "status")]
    public required string Status { get; init; }
    
    [JsonProperty(PropertyName = "paid")]
    public required bool Paid { get; init; }
}