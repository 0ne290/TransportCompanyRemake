namespace Domain.Constants;

public static class HazardClassesFlags
{
    public static bool IsFlagCombination(int value) => value is >= 0b0000_0000_0000_0000_0001 and <= 0b1111_1111_1111_1111_1111;

    public static bool IsFlag(int value) => ValidFlags.Contains(value);
    
    public static string FlagCombinationToString(int flagCombination)
    {
        if (flagCombination < 1 || !IsFlagCombination(flagCombination))
            return "Unknown flag combination";
        
        var flagCombinationString = FlagStringPairs.Where(fsp => (fsp.Value & flagCombination) > 0).Aggregate("", (current, flagStringPair) => current + $"{flagStringPair.Key} | ");

        return flagCombinationString == "" ? "Unknown flag combination" : flagCombinationString[..^3];
    }
    
    public static int StringToFlagCombination(string flagCombinationString)
    {
        flagCombinationString = flagCombinationString.Replace(" ", "");
        var flagsStrings = flagCombinationString.Split("|");
        
        var flagCombination = 0;
        foreach (var flagString in flagsStrings)
        {
            if (!FlagsByStrings.TryGetValue(flagString, out var flag))
                return 0;
            
            flagCombination |= flag;
        }

        return flagCombination;
    }
    
    public const int Class11 = 0b0000_0000_0000_0000_0001;

    public const int Class12 = 0b0000_0000_0000_0000_0010;
    
    public const int Class13 = 0b0000_0000_0000_0000_0100;
    
    public const int Class14 = 0b0000_0000_0000_0000_1000;
    
    public const int Class15 = 0b0000_0000_0000_0001_0000;
    
    public const int Class16 = 0b0000_0000_0000_0010_0000;
    
    public const int Class21 = 0b0000_0000_0000_0100_0000;
    
    public const int Class22 = 0b0000_0000_0000_1000_0000;
    
    public const int Class23 = 0b0000_0000_0001_0000_0000;
    
    public const int Class3 = 0b0000_0000_0010_0000_0000;
    
    public const int Class41 = 0b0000_0000_0100_0000_0000;
    
    public const int Class42 = 0b0000_0000_1000_0000_0000;
    
    public const int Class43 = 0b0000_0001_0000_0000_0000;
    
    public const int Class51 = 0b0000_0010_0000_0000_0000;
    
    public const int Class52 = 0b0000_0100_0000_0000_0000;
    
    public const int Class61 = 0b0000_1000_0000_0000_0000;
    
    public const int Class62 = 0b0001_0000_0000_0000_0000;
    
    public const int Class7 = 0b0010_0000_0000_0000_0000;
    
    public const int Class8 = 0b0100_0000_0000_0000_0000;
    
    public const int Class9 = 0b1000_0000_0000_0000_0000;

    private static readonly HashSet<int> ValidFlags = new(new[]
    {
        Class11, Class12, Class13, Class14, Class15, Class16, Class21, Class22, Class23, Class3, Class41, Class42,
        Class43, Class51, Class52, Class61, Class62, Class7, Class8, Class9
    });

    private static readonly IEnumerable<KeyValuePair<string, int>> FlagStringPairs = new[]
    {
        new KeyValuePair<string, int>("HazardClassesFlags.Class11", Class11),
        new KeyValuePair<string, int>("HazardClassesFlags.Class12", Class12),
        new KeyValuePair<string, int>("HazardClassesFlags.Class13", Class13),
        new KeyValuePair<string, int>("HazardClassesFlags.Class14", Class14),
        new KeyValuePair<string, int>("HazardClassesFlags.Class15", Class15),
        new KeyValuePair<string, int>("HazardClassesFlags.Class16", Class16),
        new KeyValuePair<string, int>("HazardClassesFlags.Class21", Class21),
        new KeyValuePair<string, int>("HazardClassesFlags.Class22", Class22),
        new KeyValuePair<string, int>("HazardClassesFlags.Class23", Class23),
        new KeyValuePair<string, int>("HazardClassesFlags.Class3", Class3),
        new KeyValuePair<string, int>("HazardClassesFlags.Class41", Class41),
        new KeyValuePair<string, int>("HazardClassesFlags.Class42", Class42),
        new KeyValuePair<string, int>("HazardClassesFlags.Class43", Class43),
        new KeyValuePair<string, int>("HazardClassesFlags.Class51", Class51),
        new KeyValuePair<string, int>("HazardClassesFlags.Class52", Class52),
        new KeyValuePair<string, int>("HazardClassesFlags.Class61", Class61),
        new KeyValuePair<string, int>("HazardClassesFlags.Class62", Class62),
        new KeyValuePair<string, int>("HazardClassesFlags.Class7", Class7),
        new KeyValuePair<string, int>("HazardClassesFlags.Class8", Class8),
        new KeyValuePair<string, int>("HazardClassesFlags.Class9", Class9)
    };
    
    private static readonly Dictionary<string, int> FlagsByStrings = new(FlagStringPairs);
}