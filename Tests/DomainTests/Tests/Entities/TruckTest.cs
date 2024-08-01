using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Entities;
using DomainTests.Fixtures;
using RegexFixture = DomainTests.Fixtures.RegexFixture;

namespace DomainTests.Tests.Entities;

public class TruckTest
{
    [Fact]
    public void Truck_NewWithPermittedHazardClassesFlags_ArgumentsIsValid_ReturnTheTruck_Test()
    {
        // Arrange
        var expectedBranch = BranchFixture.Create();
        var expectedWriteOnDateError = TimeSpan.FromSeconds(10);
        var expectedWriteOnDate = DateTime.Now;

        // Act
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(expectedBranch);
            
        // Assert
        Assert.Equal(expectedWriteOnDate, truck.CommissionedDate, expectedWriteOnDateError);
        Assert.Null(truck.DecommissionedDate);
        Assert.Equal(TruckFixture.DefaultVolumeMax, truck.VolumeMax);
        Assert.Equal(TruckFixture.DefaultVolumePrice, truck.VolumePrice);
        Assert.Equal(TruckFixture.DefaultWeightMax, truck.WeightMax);
        Assert.Equal(TruckFixture.DefaultWeightPrice, truck.WeightPrice);
        Assert.Equal(TruckFixture.DefaultPricePerKm, truck.PricePerKm);
        Assert.True(truck.IsAvailable);
        Assert.Equal(TruckFixture.DefaultTank, truck.TrailerIsTank);
        Assert.Equal(TruckFixture.DefaultNumber, truck.Number);
        Assert.Equal(TruckFixture.DefaultPermittedHazardClassessFlags, truck.PermittedHazardClassesFlags);
        Assert.Equal(expectedBranch, truck.Branch);
        Assert.Equal(expectedBranch.Guid, truck.BranchGuid);
        Assert.Matches(_guidRegex, truck.Guid);
    }
    
    [Fact]
    public void Truck_NewWithPermittedHazardClassesFlags_PermittedHazardClassesFlagsIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        const int permittedHazardClassesFlags = 1_048_576;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => TruckFixture.CreateWithPermittedHazardClassesFlags(BranchFixture.Create(), permittedHazardClassessFlags: permittedHazardClassesFlags));
    }
    
    [Fact]
    public void Truck_NewWithPermittedHazardClassesFlags_ArgumentsIsValid_ReturnThe100TrucksWithUniqueGuids_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var guids = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
            
            // Assert
            Assert.DoesNotContain(truck.Guid, guids);

            guids.Add(truck.Guid);
        }
    }
    
    [Fact]
    public void Truck_NewWithoutPermittedHazardClassesFlags_ArgumentsIsValid_ReturnTheTruck_Test()
    {
        // Arrange
        var expectedBranch = BranchFixture.Create();
        var expectedWriteOnDateError = TimeSpan.FromSeconds(10);
        var expectedWriteOnDate = DateTime.Now;

        // Act
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(expectedBranch);
            
        // Assert
        Assert.Equal(expectedWriteOnDate, truck.CommissionedDate, expectedWriteOnDateError);
        Assert.Null(truck.DecommissionedDate);
        Assert.Equal(TruckFixture.DefaultVolumeMax, truck.VolumeMax);
        Assert.Equal(TruckFixture.DefaultVolumePrice, truck.VolumePrice);
        Assert.Equal(TruckFixture.DefaultWeightMax, truck.WeightMax);
        Assert.Equal(TruckFixture.DefaultWeightPrice, truck.WeightPrice);
        Assert.Equal(TruckFixture.DefaultPricePerKm, truck.PricePerKm);
        Assert.True(truck.IsAvailable);
        Assert.Equal(TruckFixture.DefaultTank, truck.TrailerIsTank);
        Assert.Equal(TruckFixture.DefaultNumber, truck.Number);
        Assert.Null(truck.PermittedHazardClassesFlags);
        Assert.Equal(expectedBranch, truck.Branch);
        Assert.Equal(expectedBranch.Guid, truck.BranchGuid);
        Assert.Matches(_guidRegex, truck.Guid);
    }
    
    [Fact]
    public void Truck_NewWithoutPermittedHazardClassesFlags_ArgumentsIsValid_ReturnThe100TrucksWithUniqueGuids_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var guids = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
            
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
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(BranchFixture.Create());

        // Act
        truck.WriteOff();

        // Assert
        Assert.NotNull(truck.DecommissionedDate);
        Assert.Equal(expectedWriteOffDate, truck.DecommissionedDate.Value, expectedWriteOffDateError);
        Assert.False(truck.IsAvailable);
    }

    [Fact]
    public void Truck_Reinstate_ContextIsValid_SetTheWriteOffDateToNullAndIsAvailableToTrue_Test()
    {
        // Arrange
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(BranchFixture.Create());
        truck.WriteOff();

        // Act
        truck.Recommission();

        // Assert
        Assert.Null(truck.DecommissionedDate);
        Assert.True(truck.IsAvailable);
    }
    
    [Fact]
    public void Truck_SetPermittedHazardClassesFlags_ContextAndArgumentIsValid_SetThePermittedHazardClassesFlags_Test()
    {
        // Arrange
        const int expectedPermittedHazardClassesFlags = HazardClassesFlags.Class21 | HazardClassesFlags.Class22 | HazardClassesFlags.Class23;
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(BranchFixture.Create());

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
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(BranchFixture.Create());
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => truck.SetPermittedHazardClassesFlags(permittedHazardClassesFlags));
    }
    
    [Fact]
    public void Truck_ResetPermittedHazardClassesFlags_ContextAndArgumentIsValid_SetThePermittedHazardClassesFlagsToNull_Test()
    {
        // Arrange
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(BranchFixture.Create());

        // Act
        truck.ResetPermittedHazardClassesFlags();
        
        // Assert
        Assert.Null(truck.PermittedHazardClassesFlags);
    }
    
    [Fact]
    public void Truck_SetBranch_ContextAndArgumentIsValid_SetTheBranchAndBranchGuid_Test()
    {
        // Arrange
        var expectedBranch = Branch.New("ExpectedAddress", (13.8, -4));
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(BranchFixture.Create());

        // Act
        truck.SetBranch(expectedBranch);
        
        // Assert
        Assert.Equal(expectedBranch, truck.Branch);
        Assert.Equal(expectedBranch.Guid, truck.BranchGuid);
    }

    // Price calculation formula:
    // (Truck.WeightPrice * Order.CargoWeight + Truck.VolumePrice * Order.CargoVolume) * Truck.PricePerKm * Order.LengthInKm
    [Fact]
    public void Truck_CalculateOrderPricePerKm_ContextAndArgumentIsValid_ReturnTheOrderPrice_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var order = OrderFixture.Create(UserFixture.CreateVk());
        const decimal expectedPricePerKm =
            (TruckFixture.DefaultWeightPrice * OrderFixture.DefaultCargoWeight +
             TruckFixture.DefaultVolumePrice * OrderFixture.DefaultCargoVolume) * TruckFixture.DefaultPricePerKm;
        
        // Act
        var actualPricePerKm = truck.CalculateOrderPricePerKm(order);

        // Assert
        Assert.Equal(expectedPricePerKm, actualPricePerKm);
    }
    
    private readonly Regex _guidRegex = RegexFixture.GuidRegex();
}
