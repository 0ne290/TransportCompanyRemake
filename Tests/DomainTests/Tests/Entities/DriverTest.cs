using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Entities;

namespace DomainTests.Tests.Entities;

public partial class DriverTest
{
    [Theory]
    [InlineData(AdrDriverQualificationsFlags.Base)]
    [InlineData(AdrDriverQualificationsFlags.Full)]
    public void Driver_New_ArgumentsIsValid_ReturnTheDriverWithAdrQualification_Test(int expectedAdrQualificationFlag)
    {
        // Arrange
        var guidRegex = GuidRegex();
        var expectedBranch = Branch.New("AnyAddress", (37.314, -2.425));
        const double expectedHoursWorkedPerWeek = 0;
        const double expectedTotalHoursWorked = 0;
        const string expectedName = "AnyName";
        const bool expectedAdrQualificationOfTank = true;
        var expectedHireDateError = TimeSpan.FromSeconds(10);
        var expectedHireDate = DateTime.Now;

        // Act
        var driver = Driver.New(expectedName, expectedAdrQualificationFlag, expectedAdrQualificationOfTank, expectedBranch);
            
        // Assert
        Assert.Equal(expectedHireDate, driver.HireDate, expectedHireDateError);
        Assert.Null(driver.DismissalDate);
        Assert.Equal(expectedHoursWorkedPerWeek, driver.HoursWorkedPerWeek);
        Assert.Equal(expectedTotalHoursWorked, driver.TotalHoursWorked);
        Assert.True(driver.IsAvailable);
        Assert.Equal(expectedName, driver.Name);
        Assert.NotNull(driver.AdrQualificationFlag);
        Assert.Equal(expectedAdrQualificationFlag, driver.AdrQualificationFlag);
        Assert.Equal(expectedAdrQualificationOfTank, driver.AdrQualificationOfTank);
        Assert.Equal(expectedBranch, driver.Branch);
        Assert.Equal(expectedBranch.Guid, driver.BranchGuid);
        Assert.Matches(guidRegex, driver.Guid);
    }

    [Theory]
    [InlineData(AdrDriverQualificationsFlags.Full + 1)]
    [InlineData(AdrDriverQualificationsFlags.Base + 1)]
    public void
        Driver_New_AdrQualificationsFlagsIsInvalid_ThrowArgumentOutOfRangeException_Test(
            int adrQualificationFlag) => Assert.Throws<ArgumentOutOfRangeException>(() =>
        Driver.New("AnyName", adrQualificationFlag, true, Branch.New("AnyAddress", (37.314, -2.425))));
    
    [Fact]
    public void Driver_New_ArgumentsIsValid_ReturnTheDriverWithoutAdrQualification_Test()
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
        var driver = Driver.New(expectedName, expectedBranch);
            
        // Assert
        Assert.Equal(expectedHireDate, driver.HireDate, expectedHireDateError);
        Assert.Null(driver.DismissalDate);
        Assert.Equal(expectedHoursWorkedPerWeek, driver.HoursWorkedPerWeek);
        Assert.Equal(expectedTotalHoursWorked, driver.TotalHoursWorked);
        Assert.True(driver.IsAvailable);
        Assert.Equal(expectedName, driver.Name);
        Assert.Null(driver.AdrQualificationFlag);
        Assert.False(driver.AdrQualificationOfTank);
        Assert.Equal(expectedBranch, driver.Branch);
        Assert.Equal(expectedBranch.Guid, driver.BranchGuid);
        Assert.Matches(guidRegex, driver.Guid);
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
            var driver = Driver.New("AnyName", branch);
            
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
        var driver = Driver.New("AnyName", Branch.New("AnyAddress", (37.314, -2.425)));

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
        var driver = Driver.New("AnyName", Branch.New("AnyAddress", (37.314, -2.425)));
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
        var driver = Driver.New("AnyName", Branch.New("AnyAddress", (37.314, -2.425)));

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
        var driver = Driver.New("AnyName", Branch.New("AnyAddress", (37.314, -2.425)));
        driver.AddHoursWorked(17);

        // Act
        driver.ResetHoursWorkedPerWeek();

        // Assert
        Assert.Equal(expectedHoursWorkedPerWeek, driver.HoursWorkedPerWeek);
    }
    
    [Fact]
    public void Driver_DequalifyAdr_ContextIsValid_SetTheAdrQualificationOfTankToFalseAndAdrQualificationFlagToNull_Test()
    {
        // Arrange
        var driver = Driver.New("AnyName", AdrDriverQualificationsFlags.Class7, true, Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        driver.DequalifyAdr();
            
        // Assert
        Assert.False(driver.AdrQualificationOfTank);
        Assert.Null(driver.AdrQualificationFlag);
    }
    
    [Theory]
    [InlineData(AdrDriverQualificationsFlags.Base)]
    [InlineData(AdrDriverQualificationsFlags.Full)]
    public void Driver_QualifyAdr_ContextAndArgumentIsValid_SetTheAdrQualificationFlag_Test(int expectedAdrQualificationFlag)
    {
        // Arrange
        var driver = Driver.New("AnyName", Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        driver.QualifyAdr(expectedAdrQualificationFlag);
            
        // Assert
        Assert.Equal(expectedAdrQualificationFlag, driver.AdrQualificationFlag);
    }
    
    [Theory]
    [InlineData(AdrDriverQualificationsFlags.Full + 1)]
    [InlineData(AdrDriverQualificationsFlags.Base + 1)]
    public void Driver_QualifyAdr_AdrQualificationFlagIsInvalid_ThrowArgumentOutOfRangeException_Test(int adrQualificationsFlags)
    {
        // Arrange
        var driver = Driver.New("AnyName", Branch.New("AnyAddress", (37.314, -2.425)));

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => driver.QualifyAdr(adrQualificationsFlags));
    }
    
    [Fact]
    public void Driver_QualifyAdrTank_ContextIsValid_SetTheAdrQualificationOfTankToTrue_Test()
    {
        // Arrange
        var driver = Driver.New("AnyName", AdrDriverQualificationsFlags.Base, false, Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        driver.QualifyAdrTank();
            
        // Assert
        Assert.True(driver.AdrQualificationOfTank);
    }
    
    [Fact]
    public void Driver_QualifyAdrTank_AdrQualificationFlagIsInvalid_ThrowInvalidOperationException_Test()
    {
        // Arrange
        var driver = Driver.New("AnyName", Branch.New("AnyAddress", (37.314, -2.425)));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => driver.QualifyAdrTank());
    }
    
    [Fact]
    public void Driver_DequalifyAdrTank_ContextIsValid_SetTheAdrQualificationOfTankToFalse_Test()
    {
        // Arrange
        var driver = Driver.New("AnyName", AdrDriverQualificationsFlags.Base, true, Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        driver.DequalifyAdrTank();
            
        // Assert
        Assert.False(driver.AdrQualificationOfTank);
    }
    
    [Fact]
    public void Driver_SetBranch_ContextAndArgumentIsValid_SetTheBranchAndBranchGuid_Test()
    {
        // Arrange
        var expectedBranch = Branch.New("ExpectedAddress", (13.8, -4));
        var driver = Driver.New("AnyName", Branch.New("StubAddress", (37.314, -2.425)));

        // Act
        driver.SetBranch(expectedBranch);
        
        // Assert
        Assert.Equal(expectedBranch, driver.Branch);
        Assert.Equal(expectedBranch.Guid, driver.BranchGuid);
    }
    
    [GeneratedRegex(@"^(?i)[a-z\d]{8}-([a-z\d]{4}-){3}[a-z\d]{12}$", RegexOptions.None, "ru-RU")]
    private static partial Regex GuidRegex();
}
