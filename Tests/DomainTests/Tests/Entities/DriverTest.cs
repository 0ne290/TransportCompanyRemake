using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Entities;
using DomainTests.Tests.Fixtures;

namespace DomainTests.Tests.Entities;

public class DriverTest
{
    [Fact]
    public void Driver_NewDriverWithAdrQualification_ArgumentsIsValid_ReturnTheDriver_Test()
    {
        // Arrange
        var expectedBranch = BranchFixture.Create();
        const double expectedHoursWorkedPerWeek = 0;
        const double expectedTotalHoursWorked = 0;
        var expectedHireDateError = TimeSpan.FromSeconds(10);
        var expectedHireDate = DateTime.Now;

        // Act
        var driver = DriverFixture.CreateWithAdrQualificationFlag(expectedBranch);
            
        // Assert
        Assert.Equal(expectedHireDate, driver.HireDate, expectedHireDateError);
        Assert.Null(driver.DismissalDate);
        Assert.Equal(expectedHoursWorkedPerWeek, driver.HoursWorkedPerWeek);
        Assert.Equal(expectedTotalHoursWorked, driver.TotalHoursWorked);
        Assert.True(driver.IsAvailable);
        Assert.Equal(DriverFixture.DefaultName, driver.Name);
        Assert.NotNull(driver.AdrQualificationFlag);
        Assert.Equal(DriverFixture.DefaultAdrDriverQualificationFlag, driver.AdrQualificationFlag);
        Assert.Equal(DriverFixture.DefaultAdrQualificationOfTank, driver.AdrQualificationOfTank);
        Assert.Equal(expectedBranch, driver.Branch);
        Assert.Equal(expectedBranch.Guid, driver.BranchGuid);
        Assert.Matches(_guidRegex, driver.Guid);
    }

    [Fact]
    public void Driver_NewDriverWithAdrQualification_AdrQualificationsFlagsIsInvalid_ThrowArgumentOutOfRangeException_Test() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DriverFixture.CreateWithAdrQualificationFlag(BranchFixture.Create(),
                adrQualificationFlag: AdrDriverQualificationsFlags.Base + 1));
    
    [Fact]
    public void Driver_NewDriverWithAdrQualificationFlagDriver_ArgumentsIsValid_ReturnThe100DriversWithUniqueGuids_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var guids = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var driver = DriverFixture.CreateWithAdrQualificationFlag(branch);
            
            // Assert
            Assert.DoesNotContain(driver.Guid, guids);

            guids.Add(driver.Guid);
        }
    }

    [Fact]
    public void Driver_NewDriverWithoutAdrQualification_ArgumentsIsValid_ReturnTheDriver_Test()
    {
        // Arrange
        var expectedBranch = BranchFixture.Create();
        const double expectedHoursWorkedPerWeek = 0;
        const double expectedTotalHoursWorked = 0;
        var expectedHireDateError = TimeSpan.FromSeconds(10);
        var expectedHireDate = DateTime.Now;

        // Act
        var driver = DriverFixture.CreateWithoutAdrQualificationFlag(expectedBranch);
            
        // Assert
        Assert.Equal(expectedHireDate, driver.HireDate, expectedHireDateError);
        Assert.Null(driver.DismissalDate);
        Assert.Equal(expectedHoursWorkedPerWeek, driver.HoursWorkedPerWeek);
        Assert.Equal(expectedTotalHoursWorked, driver.TotalHoursWorked);
        Assert.True(driver.IsAvailable);
        Assert.Equal(DriverFixture.DefaultName, driver.Name);
        Assert.Null(driver.AdrQualificationFlag);
        Assert.False(driver.AdrQualificationOfTank);
        Assert.Equal(expectedBranch, driver.Branch);
        Assert.Equal(expectedBranch.Guid, driver.BranchGuid);
        Assert.Matches(_guidRegex, driver.Guid);
    }
    
    [Fact]
    public void Driver_NewDriverWithoutAdrQualification_ArgumentsIsValid_ReturnThe100DriversWithUniqueGuids_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var guids = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var driver = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
            
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
        var driver = DriverFixture.CreateWithoutAdrQualificationFlag(BranchFixture.Create());

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
        var driver = DriverFixture.CreateWithoutAdrQualificationFlag(BranchFixture.Create());
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
        var driver = DriverFixture.CreateWithoutAdrQualificationFlag(BranchFixture.Create());

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
        var driver = DriverFixture.CreateWithoutAdrQualificationFlag(BranchFixture.Create());
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
        var driver = DriverFixture.CreateWithAdrQualificationFlag(BranchFixture.Create(), adrQualificationOfTank: true);

        // Act
        driver.DequalifyAdr();
            
        // Assert
        Assert.False(driver.AdrQualificationOfTank);
        Assert.Null(driver.AdrQualificationFlag);
    }
    
    [Fact]
    public void Driver_QualifyAdr_ContextAndArgumentIsValid_SetTheAdrQualificationFlag_Test()
    {
        // Arrange
        var expectedAdrQualificationFlag = AdrDriverQualificationsFlags.Full;
        var driver = DriverFixture.CreateWithoutAdrQualificationFlag();

        // Act
        driver.QualifyAdr(expectedAdrQualificationFlag);
            
        // Assert
        Assert.Equal(expectedAdrQualificationFlag, driver.AdrQualificationFlag);
    }
    
    [Fact]
    public void Driver_QualifyAdr_AdrQualificationFlagIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        var adrQualificationFlag = AdrDriverQualificationsFlags.Base + 1;
        var driver = DriverFixture.CreateWithoutAdrQualificationFlag(BranchFixture.Create());

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => driver.QualifyAdr(adrQualificationFlag));
    }
    
    [Fact]
    public void Driver_QualifyAdrTank_ContextIsValid_SetTheAdrQualificationOfTankToTrue_Test()
    {
        // Arrange
        var driver = DriverFixture.CreateWithAdrQualificationFlag(BranchFixture.Create(), adrQualificationOfTank: false);

        // Act
        driver.QualifyAdrTank();
            
        // Assert
        Assert.True(driver.AdrQualificationOfTank);
    }
    
    [Fact]
    public void Driver_QualifyAdrTank_AdrQualificationFlagIsInvalid_ThrowInvalidOperationException_Test()
    {
        // Arrange
        var driver = DriverFixture.CreateWithoutAdrQualificationFlag(BranchFixture.Create());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => driver.QualifyAdrTank());
    }
    
    [Fact]
    public void Driver_DequalifyAdrTank_ContextIsValid_SetTheAdrQualificationOfTankToFalse_Test()
    {
        // Arrange
        var driver = DriverFixture.CreateWithAdrQualificationFlag(BranchFixture.Create(), adrQualificationOfTank: true);

        // Act
        driver.DequalifyAdrTank();
            
        // Assert
        Assert.False(driver.AdrQualificationOfTank);
    }
    
    [Fact]
    public void Driver_SetBranch_ContextAndArgumentIsValid_SetTheBranchAndBranchGuid_Test()
    {
        // Arrange
        var expectedBranch = BranchFixture.Create();
        var driver = DriverFixture.CreateWithoutAdrQualificationFlag(BranchFixture.Create());

        // Act
        driver.SetBranch(expectedBranch);
        
        // Assert
        Assert.Equal(expectedBranch, driver.Branch);
        Assert.Equal(expectedBranch.Guid, driver.BranchGuid);
    }
    
    private readonly Regex _guidRegex = RegexFixture.GuidRegex();
}
