using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Entities;

namespace DomainTests.Tests.Entities;

public partial class DriverTest
{
    [Fact]
    public void New_AdrDriverQualificationsFlagsIsOne_ReturnTheDriver_Test()
    {
        // Arrange
        var guidRegex = GuidRegex();
        var expectedBranch = Branch.New("AnyAddress", (37.314, -2.425));
        const double expectedHoursWorkedPerWeek = 0;
        const double expectedTotalHoursWorked = 0;
        const bool expectedIsAvailable = true;
        const string expectedName = "AnyName";
        const int expectedAdrQualificationsFlags = AdrDriverQualificationsFlags.Base;
        DateTime? expectedDismissalDate = null;
        var expectedHireDateError = TimeSpan.FromSeconds(10);
        var expectedHireDate = DateTime.Now;

        // Act
        var driver = Driver.New(expectedName, expectedAdrQualificationsFlags, expectedBranch);
            
        // Assert
        Assert.Equal(expectedHireDate, driver.HireDate, expectedHireDateError);
        Assert.Equal(expectedDismissalDate, driver.DismissalDate);
        Assert.Equal(expectedHoursWorkedPerWeek, driver.HoursWorkedPerWeek);
        Assert.Equal(expectedTotalHoursWorked, driver.TotalHoursWorked);
        Assert.Equal(expectedIsAvailable, driver.IsAvailable);
        Assert.Equal(expectedName, driver.Name);
        Assert.Equal(expectedAdrQualificationsFlags, driver.AdrQualificationsFlags);
        Assert.Equal(expectedBranch, driver.Branch);
        Assert.Equal(expectedBranch.Guid, driver.BranchGuid);
        Assert.Matches(guidRegex, driver.Guid);
    }
    
    [Fact]
    public void New_AdrDriverQualificationsFlagsIsMultiple_ReturnTheDriver_Test()
    {
        // Arrange
        const int expectedAdrQualificationsFlags = AdrDriverQualificationsFlags.Class17 | AdrDriverQualificationsFlags.Class18;

        // Act
        var driver = Driver.New("AnyName", expectedAdrQualificationsFlags, Branch.New("AnyAddress", (37.314, -2.425)));
            
        // Assert
        Assert.Equal(expectedAdrQualificationsFlags, driver.AdrQualificationsFlags);
    }
    
    [Fact]
    public void New_AdrDriverQualificationsFlagsIsNull_ReturnTheDriver_Test()
    {
        // Arrange
        int? expectedAdrQualificationsFlags = null;

        // Act
        var driver = Driver.New("AnyName", expectedAdrQualificationsFlags, Branch.New("AnyAddress", (37.314, -2.425)));
            
        // Assert
        Assert.Equal(expectedAdrQualificationsFlags, driver.AdrQualificationsFlags);
    }
    
    [Fact]
    public void New_AdrDriverQualificationsFlagsIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        const int adrQualificationsFlags = 10;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Driver.New("AnyName", adrQualificationsFlags, Branch.New("AnyAddress", (37.314, -2.425))));
    }
    
    [GeneratedRegex(@"^(?i)[a-z\d]{8}-([a-z\d]{4}-){3}[a-z\d]{12}$", RegexOptions.None, "ru-RU")]
    private static partial Regex GuidRegex();
}