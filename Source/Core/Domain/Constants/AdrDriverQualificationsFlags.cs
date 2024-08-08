namespace Domain.Constants;

public static class AdrDriverQualificationsFlags
{
    public static bool IsFlag(int flag) => ValidFlags.Contains(flag);

    public static string FlagToString(int flag) => StringsByFlags.TryGetValue(flag, out var flagString) ? flagString : "Unknown flag";
    
    public static int StringToFlag(string flagString) => FlagsByStrings.TryGetValue(flagString, out var flag) ? flag : 0;

    public const int Base = HazardClassesFlags.Class21 | HazardClassesFlags.Class22 | HazardClassesFlags.Class23 |
                            HazardClassesFlags.Class3 | HazardClassesFlags.Class41 | HazardClassesFlags.Class42 |
                            HazardClassesFlags.Class43 | HazardClassesFlags.Class51 | HazardClassesFlags.Class52 |
                            HazardClassesFlags.Class61 | HazardClassesFlags.Class62 | HazardClassesFlags.Class8 |
                            HazardClassesFlags.Class9;

    public const int BaseAndClass7 = Base | HazardClassesFlags.Class7;

    public const int BaseAndClass1 = Base | HazardClassesFlags.Class11 | HazardClassesFlags.Class12 |
                                     HazardClassesFlags.Class13 | HazardClassesFlags.Class14 |
                                     HazardClassesFlags.Class15 | HazardClassesFlags.Class16;

    public const int Full = BaseAndClass7 | BaseAndClass1;

    private static readonly HashSet<int> ValidFlags = new(new[] { Base, BaseAndClass7, BaseAndClass1, Full });

    private static readonly Dictionary<string, int> FlagsByStrings = new(new[]
    {
        new KeyValuePair<string, int>("AdrDriverQualificationsFlags.Base", Base), new KeyValuePair<string, int>("AdrDriverQualificationsFlags.BaseAndClass7", BaseAndClass7),
        new KeyValuePair<string, int>("AdrDriverQualificationsFlags.BaseAndClass1", BaseAndClass1), new KeyValuePair<string, int>("AdrDriverQualificationsFlags.Full", Full)
    });
    
    private static readonly Dictionary<int, string> StringsByFlags = new(new[]
    {
        new KeyValuePair<int, string>(Base, "AdrDriverQualificationsFlags.Base"), new KeyValuePair<int, string>(BaseAndClass7, "AdrDriverQualificationsFlags.BaseAndClass7"),
        new KeyValuePair<int, string>(BaseAndClass1, "AdrDriverQualificationsFlags.BaseAndClass1"), new KeyValuePair<int, string>(Full, "AdrDriverQualificationsFlags.Full")
    });
}