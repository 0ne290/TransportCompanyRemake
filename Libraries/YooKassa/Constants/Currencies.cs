namespace YooKassa.Constants;

public static class Currencies
{
    public static bool IsCurrency(string currency) => ValidCurrencies.Contains(currency);
    
    public const string Rub = "RUB";
    
    private static readonly HashSet<string> ValidCurrencies = new(new[] { Rub });
}