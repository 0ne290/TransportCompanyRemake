namespace YooKassa.Constants;

public static class ConfirmationTypes
{
    public static bool IsType(string type) => ValidTypes.Contains(type);
    
    /*public const string Embedded = "embedded";
    
    public const string External = "external";
    
    public const string MobileApplication = "mobile_application";
    
    public const string Qr = "qr";*/
    
    public const string Redirect = "redirect";

    private static readonly HashSet<string> ValidTypes = new(new[]
        { /*Embedded, External, MobileApplication, Qr, */Redirect });
}