namespace Domain.Constants;

public static class AdrDriverQualificationsFlags
{
    public static bool IsFlagCombination(int value) => value is >= 0b0001 and <= 0b1111;

    public static bool IsFlag(int value) => Flags.Contains(value);
    
    public const int Base = 0b0001;

    public const int Class7 = 0b0011;
    
    public const int Class8 = 0b0101;
    
    public const int Tank = 0b1001;

    private static readonly HashSet<int> Flags = new(new[] { Base, Class7, Class8, Tank });
}