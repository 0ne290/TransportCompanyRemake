namespace Domain.Constants;

public static class AdrDriverQualificationsFlags
{
    public static bool IsFlagCombination(int value) => value is >= Base and <= Tank;

    public static bool IsFlag(int value) => _flags.Contains(value);
    
    public const int Base = 0b0001;

    public const int Class17 = 0b0011;
    
    public const int Class18 = 0b0101;
    
    public const int Tank = 0b1001;

    private static HashSet<int> _flags = new(new[] { Base, Class17, Class18, Tank });
}