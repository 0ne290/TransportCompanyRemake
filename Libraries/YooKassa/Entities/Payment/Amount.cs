using System.Globalization;
using Newtonsoft.Json;
using YooKassa.Constants;

namespace YooKassa.Entities.Payment;

public class Amount
{
    [JsonProperty(PropertyName = "value")]
    public required string Value
    {
        get => _value;
        init
        {
            if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value is invalid.");

            _value = value;
        }
    }

    [JsonProperty(PropertyName = "currency")]
    public required string Currency
    {
        get => _currency;
        init
        {
            if (!Currencies.IsCurrency(value))
                throw new ArgumentOutOfRangeException(nameof(value), value, "Currency is invalid.");
            
            _currency = value;
        }
    }
    
    private readonly string _value = null!;

    private readonly string _currency = null!;
}