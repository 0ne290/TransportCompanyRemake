using System.Text.RegularExpressions;

namespace DomainTests.Tests.Fixtures;

public static partial class RegexFixture
{
    [GeneratedRegex(@"^(?i)[a-z\d]{8}-([a-z\d]{4}-){3}[a-z\d]{12}$", RegexOptions.None, "ru-RU")]
    public static partial Regex GuidRegex();
    
    [GeneratedRegex(@"^(?i)[a-z\d]{128}$", RegexOptions.None, "ru-RU")]
    public static partial Regex DynamicPartOfSaltRegex();
}