using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Entities;

namespace DomainTests.Tests.Entities;

public partial class DriverTest
{
    [Theory]
    [InlineData(AdrDriverQualificationsFlags.Base)]
    [InlineData(AdrDriverQualificationsFlags.Class17 | AdrDriverQualificationsFlags.Class18)]
    [InlineData(null)]
    public void New_AdrDriverQualificationsFlagsIsValid_ReturnTheDriver_Test(int? expectedAdrQualificationsFlags)
    {
        // Arrange
        var guidRegex = GuidRegex();
        var expectedBranch = Branch.New("AnyAddress", (37.314, -2.425));
        const double expectedHoursWorkedPerWeek = 0;
        const double expectedTotalHoursWorked = 0;
        const bool expectedIsAvailable = true;
        const string expectedName = "AnyName";
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