using YooKassa.Constants;

namespace YooKassa.Entities.Payment;

public class Amount
{
    public Amount(decimal value, string currency)
    {
        if (!Currencies.IsCurrency(currency))
            throw new ArgumentOutOfRangeException(nameof(currency), currency, "Currency is invalid.");

        Value = value;
        Currency = currency;
    }
    
    public decimal Value { get; private set; }
    
    public string Currency { get; private set; }
}