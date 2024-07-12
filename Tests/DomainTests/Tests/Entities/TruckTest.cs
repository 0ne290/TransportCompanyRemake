using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Entities;

namespace DomainTests.Tests.Entities;

public partial class TruckTest
{
    [Theory]
    [InlineData(HazardClassesFlags.Class3)]
    [InlineData(HazardClassesFlags.Class61 | HazardClassesFlags.Class62)]
    [InlineData(null)]
    public void Truck_New_ArgumentsIsValid_ReturnTheTruck_Test(int? expectedPermittedHazardClassesFlags)
    {
        // Arrange
        var guidRegex = GuidRegex();
        var expectedBranch = Branch.New("AnyAddress", (37.314, -2.425));
        const bool expectedTank = true;
        const decimal expectedVolumeMax = 80;
        const decimal expectedVolumePrice = 1.5m;
        const decimal expectedWeightMax = 10008.4m;
        const decimal expectedWeightPrice = 0.7m;
        const decimal expectedPricePerKm = 1.1m;
        const string expectedNumber = "С150ТО";
        var expectedWriteOnDateError = TimeSpan.FromSeconds(10);
        var expectedWriteOnDate = DateTime.Now;

        // Act
        var truck = Truck.New(expectedNumber, expectedTank, expectedVolumeMax, expectedVolumePrice, expectedWeightMax,
            expectedWeightPrice, expectedPricePerKm, expectedPermittedHazardClassesFlags, expectedBranch);
            
        // Assert
        Assert.Equal(expectedWriteOnDate, truck.WriteOnDate, expectedWriteOnDateError);
        Assert.Null(truck.WriteOffDate);
        Assert.Equal(expectedVolumeMax, truck.VolumeMax);
        Assert.Equal(expectedVolumePrice, truck.VolumePrice);
        Assert.Equal(expectedWeightMax, truck.WeightMax);
        Assert.Equal(expectedWeightPrice, truck.WeightPrice);
        Assert.Equal(expectedPricePerKm, truck.PricePerKm);
        Assert.True(truck.IsAvailable);
        Assert.Equal(expectedNumber, truck.Number);
        Assert.Equal(expectedPermittedHazardClassesFlags, truck.PermittedHazardClassesFlags);
        Assert.Equal(expectedBranch, truck.Branch);
        Assert.Equal(expectedBranch.Guid, truck.BranchGuid);
        Assert.Matches(guidRegex, truck.Guid);
    }
    
    [Fact]
    public void Truck_New_PermittedHazardClassesFlagsIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        const int permittedHazardClassesFlags = 1_048_576;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Truck.New("С150ТО", true, 80, 1.5m, 1000.8m,0.7m, 1.1m, permittedHazardClassesFlags, Branch.New("AnyAddress", (37.314, -2.425))));
    }
    
    [Fact]
    public void Truck_New_ArgumentsIsValid_ReturnThe100TrucksWithUniqueGuids_Test()
    {
        // Arrange
        var branch = Branch.New("AnyAddress", (37.314, -2.425));
        var guids = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var truck = Truck.New("С150ТО", true, 80, 1.5m, 1000.8m, 0.7m, 1.1m, null, branch);
            
            // Assert
            Assert.DoesNotContain(truck.Guid, guids);

            guids.Add(truck.Guid);
        }
    }

    [Fact]
    public void Truck_WriteOff_ContextIsValid_SetTheWriteOffDateToNowAndIsAvailableToFalse_Test()
    {
        // Arrange
        var expectedWriteOffDateError = TimeSpan.FromSeconds(10);
        var expectedWriteOffDate = DateTime.Now;
        var truck = Truck.New("С150ТО", true, 80, 1.5m, 1000.8m, 0.7m, 1.1m, null,
            Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        truck.WriteOff();

        // Assert
        Assert.NotNull(truck.WriteOffDate);
        Assert.Equal(expectedWriteOffDate, truck.WriteOffDate.Value, expectedWriteOffDateError);
        Assert.False(truck.IsAvailable);
    }

    [Fact]
    public void Truck_Reinstate_ContextIsValid_SetTheWriteOffDateToNullAndIsAvailableToTrue_Test()
    {
        // Arrange
        var truck = Truck.New("С150ТО", true, 80, 1.5m, 1000.8m, 0.7m, 1.1m, null,
            Branch.New("AnyAddress", (37.314, -2.425)));
        truck.WriteOff();

        // Act
        truck.Reinstate();

        // Assert
        Assert.Null(truck.WriteOffDate);
        Assert.True(truck.IsAvailable);
    }
    
    [Theory]
    [InlineData(HazardClassesFlags.Class7)]
    [InlineData(HazardClassesFlags.Class11 | HazardClassesFlags.Class7 | HazardClassesFlags.Class9)]
    [InlineData(null)]
    public void Truck_SetPermittedHazardClassesFlags_ContextAndArgumentIsValid_SetThePermittedHazardClassesFlags_Test(int? expectedPermittedHazardClassesFlags)
    {
        // Arrange
        var truck = Truck.New("С150ТО", true, 80, 1.5m, 1000.8m, 0.7m, 1.1m, null,
            Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        truck.SetPermittedHazardClassesFlags(expectedPermittedHazardClassesFlags);
            
        // Assert
        Assert.Equal(expectedPermittedHazardClassesFlags, truck.PermittedHazardClassesFlags);
    }
    
    [Fact]
    public void Truck_SetPermittedHazardClassesFlags_PermittedHazardClassesFlagsIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        const int permittedHazardClassesFlags = 1_048_576;
        var truck = Truck.New("С150ТО", true, 80, 1.5m, 1000.8m,0.7m, 1.1m, null, Branch.New("AnyAddress", (37.314, -2.425)));

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => truck.SetPermittedHazardClassesFlags(permittedHazardClassesFlags));
    }
    
    [Fact]
    public void Truck_SetBranch_ContextAndArgumentIsValid_SetTheBranchAndBranchGuid_Test()
    {
        // Arrange
        var expectedBranch = Branch.New("ExpectedAddress", (13.8, -4));
        var truck = Truck.New("С150ТО", true, 80, 1.5m, 1000.8m, 0.7m, 1.1m, null,
            Branch.New("AnyAddress", (37.314, -2.425)));

        // Act
        truck.SetBranch(expectedBranch);
        
        // Assert
        Assert.Equal(expectedBranch, truck.Branch);
        Assert.Equal(expectedBranch.Guid, truck.BranchGuid);
    }

    // Price calculation formula:
    // (Truck.WeightPrice * Order.CargoWeight + Truck.VolumePrice * Order.CargoVolume) * Truck.PricePerKm * Order.DistanceInKm
    [Fact]
    public void Truck_CalculateOrderPrice_ContextAndArgumentIsValid_ReturnThePriceForFulfillingAnOrderByTruck_Test()
    {
        Assert.True(false);
    }
    
    [GeneratedRegex(@"^(?i)[a-z\d]{8}-([a-z\d]{4}-){3}[a-z\d]{12}$", RegexOptions.None, "ru-RU")]
    private static partial Regex GuidRegex();
}
