namespace Domain.Constants;

public static class AdrDriverQualificationsFlags
{
    public static bool IsFlag(int value) => Flags.Contains(value);

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

    private static readonly HashSet<int> Flags = new(new[] { Base, BaseAndClass7, BaseAndClass1, Full });
}