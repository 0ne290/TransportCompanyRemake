using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Entities;

namespace DomainTests.Tests.Entities;

public partial class DriverTest
{
    [Theory]
    [InlineData(AdrDriverQualificationsFlags.Base)]
    [InlineData(AdrDriverQualificationsFlags.Class7 | AdrDriverQualificationsFlags.Class8)]
    [InlineData(null)]
    public void Driver_New_ArgumentsIsValid_ReturnTheDriver_Test(int? expectedAdrQualificationsFlags)
    {
        // Arrange
        var guidRegex = GuidRegex();
        var expectedBranch = Branch.New("AnyAddress", (37.314, -2.425));
        const double expectedHoursWorkedPerWeek = 0;
        const double expectedTotalHoursWorked = 0;
        const string expectedName = "AnyName";
        var expectedHireDateError = TimeSpan.FromSeconds(10);
        var expectedHireDate = DateTime.Now;

        // Act
        var driver = Driver.New(expectedName, expectedAdrQualificationsFlags, expectedBranch);
            
        // Assert
        Assert.Equal(expectedHireDate, driver.HireDate, expectedHireDateError);
        Assert.Null(driver.DismissalDate);
        Assert.Equal(expectedHoursWorkedPerWeek, driver.HoursWorkedPerWeek);
        Assert.Equal(expectedTotalHoursWorked, driver.TotalHoursWorked);
        Assert.True(driver.IsAvailable);
        Assert.Equal(expectedName, driver.Name);
        Assert.Equal(expectedAdrQualificationsFlags, driver.AdrQualificationsFlags);
        Assert.Equal(expectedBranch, driver.Branch);
        Assert.Equal(expectedBranch.Guid, driver.BranchGuid);
        Assert.Matches(guidRegex, driver.Guid);
    }
    
    [Fact]
    public void Driver_New_AdrDriverQualificationsFlagsIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        const int adrQualificationsFlags = 16;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Driver.New("AnyName", adrQualificationsFlags, Branch.New("AnyAddress", (37.314, -2.425))));
    }
    
    [Fact]
    public void Driver_New_ArgumentsIsValid_ReturnThe100DriversWithUniqueGuids_Test()
    {
        // Arrange
        var branch = Branch.New("AnyAddress", (37.314, -2.425));
        var guids = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var driver = Driver.New("AnyName", null, branch);
            
            // Assert
            Assert.DoesNotContain(driver.Guid, guids);

            guids.Add(driver.Guid);
        }
    }

    [Fact]
    public void Driver_Dismiss_ContextIsValid_SetTheDismissalDateToNowAndIsAvailableToFalse_Test()
    {
        // Arrange
        var expectedDismissalDateError = TimeSpan.FromSeconds(10);
        var expectedDismissalDate = DateTime.Now;
        var driver = Driver.New("AnyName", null, Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        driver.Dismiss();

        // Assert
        Assert.NotNull(driver.DismissalDate);
        Assert.Equal(expectedDismissalDate, driver.DismissalDate.Value, expectedDismissalDateError);
        Assert.False(driver.IsAvailable);
    }

    [Fact]
    public void Driver_Reinstate_ContextIsValid_SetTheDismissalDateToNullAndIsAvailableToTrue_Test()
    {
        // Arrange
        var driver = Driver.New("AnyName", null, Branch.New("AnyAddress", (37.314, -2.425)));
        driver.Dismiss();

        // Act
        driver.Reinstate();

        // Assert
        Assert.Null(driver.DismissalDate);
        Assert.True(driver.IsAvailable);
    }
    
    [Fact]
    public void Driver_AddHoursWorked_ContextAndArgumentIsValid_IncreaseTheHoursWorkedPerWeekAndTotalHoursWorked_Test()
    {
        // Arrange
        const int increment = 17;
        var driver = Driver.New("AnyName", null, Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        driver.AddHoursWorked(increment);

        // Assert
        Assert.Equal(increment, driver.HoursWorkedPerWeek);
        Assert.Equal(increment, driver.TotalHoursWorked);
    }
    
    [Fact]
    public void Driver_ResetHoursWorkedPerWeek_ContextIsValid_SetTheHoursWorkedPerWeekToZero_Test()
    {
        // Arrange
        const int expectedHoursWorkedPerWeek = 0;
        var driver = Driver.New("AnyName", null, Branch.New("AnyAddress", (37.314, -2.425)));
        driver.AddHoursWorked(17);

        // Act
        driver.ResetHoursWorkedPerWeek();

        // Assert
        Assert.Equal(expectedHoursWorkedPerWeek, driver.HoursWorkedPerWeek);
    }
    
    [Theory]
    [InlineData(AdrDriverQualificationsFlags.Base)]
    [InlineData(AdrDriverQualificationsFlags.Class7 | AdrDriverQualificationsFlags.Class8)]
    [InlineData(null)]
    public void Driver_SetAdrQualificationsFlags_ContextAndArgumentIsValid_SetTheAdrQualificationsFlags_Test(int? expectedAdrQualificationsFlags)
    {
        // Arrange
        var driver = Driver.New("AnyName", AdrDriverQualificationsFlags.Tank, Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        driver.SetAdrQualificationsFlags(expectedAdrQualificationsFlags);
            
        // Assert
        Assert.Equal(expectedAdrQualificationsFlags, driver.AdrQualificationsFlags);
    }
    
    [Fact]
    public void Driver_SetAdrQualificationsFlags_AdrDriverQualificationsFlagsIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        const int adrQualificationsFlags = 16;
        var driver = Driver.New("AnyName", null, Branch.New("AnyAddress", (37.314, -2.425)));

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => driver.SetAdrQualificationsFlags(adrQualificationsFlags));
    }
    
    [Fact]
    public void Driver_SetBranch_ContextAndArgumentIsValid_SetTheBranchAndBranchGuid_Test()
    {
        // Arrange
        var expectedBranch = Branch.New("ExpectedAddress", (13.8, -4));
        var driver = Driver.New("AnyName", null, Branch.New("StubAddress", (37.314, -2.425)));

        // Act
        driver.SetBranch(expectedBranch);
        
        // Assert
        Assert.Equal(expectedBranch, driver.Branch);
        Assert.Equal(expectedBranch.Guid, driver.BranchGuid);
    }
    
    [GeneratedRegex(@"^(?i)[a-z\d]{8}-([a-z\d]{4}-){3}[a-z\d]{12}$", RegexOptions.None, "ru-RU")]
    private static partial Regex GuidRegex();
}
