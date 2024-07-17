using Domain.Constants;
using Domain.Entities;

namespace DomainTests.Tests.Fixtures;

public static class DriverFixture
{
    public static Driver CreateWithAdrQualificationFlag(Branch branch, string name = DefaultName,
        int adrQualificationFlag = DefaultAdrDriverQualificationFlag,
        bool adrQualificationOfTank = DefaultAdrQualificationOfTank) =>
        Driver.New(name, adrQualificationFlag, adrQualificationOfTank, branch);
    
    public static Driver CreateWithoutAdrQualificationFlag(Branch branch, string name = DefaultName) =>
        Driver.New(name, branch);
    
    public const string DefaultName = "AnyDriver1Name";
    
    public const int DefaultAdrDriverQualificationFlag = AdrDriverQualificationsFlags.BaseAndClass8;
    
    public const bool DefaultAdrQualificationOfTank = true;
}