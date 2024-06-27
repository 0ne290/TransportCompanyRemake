namespace Domain.Constants;

public static class ClassesAdr
{
    public static bool ClassExists(decimal classAdr) =>
        classAdr is None or ExplosiveSubstances or Gases or ToxicGases or FlammableLiquids
            or FlammableSolidsSelfReactiveSubstancesAndSolidDesensitizedExplosives
            or SubstancesLiableToSpontaneousCombustion or SubstancesWhichInContactWithWaterEmitFlammableGases
            or OxidizingSubstances or ToxicSubstances or CorrosiveSubstances
            or MiscellaneousDangerousSubstancesAndArticles;
    
    public const decimal None = 0;
    
    public const decimal ExplosiveSubstances = 1;
    
    public const decimal Gases = 2;
    
    public const decimal ToxicGases = 2.3m;
    
    public const decimal FlammableLiquids = 3;
    
    public const decimal FlammableSolidsSelfReactiveSubstancesAndSolidDesensitizedExplosives = 4.1m;
    
    public const decimal SubstancesLiableToSpontaneousCombustion = 4.2m;
    
    public const decimal SubstancesWhichInContactWithWaterEmitFlammableGases = 4.3m;
    
    public const decimal OxidizingSubstances = 5.1m;
    
    public const decimal ToxicSubstances = 6.1m;
    
    public const decimal CorrosiveSubstances = 8;
    
    public const decimal MiscellaneousDangerousSubstancesAndArticles = 9;
}