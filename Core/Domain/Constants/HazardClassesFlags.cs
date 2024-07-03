namespace Domain.Constants;

public static class HazardClassesFlags
{
    public static bool IsFlagCombination(int value) => value is >= Class11 and <= Class9;

    public static bool IsFlag(int value) => Flags.Contains(value);
    
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

    private static readonly HashSet<int> Flags = new(new[]
    {
        Class11, Class12, Class13, Class14, Class15, Class16, Class21, Class22, Class23, Class3, Class41, Class42,
        Class43, Class51, Class52, Class61, Class62, Class7, Class8, Class9
    });
}