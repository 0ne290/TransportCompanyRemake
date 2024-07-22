using System.Text.RegularExpressions;
using Domain.Dtos;
using DomainTests.Fixtures;
using DomainTests.Stubs;
using RegexFixture = DomainTests.Fixtures.RegexFixture;

namespace DomainTests.Tests.Entities;

public class BranchTest
{
    [Fact]
    public void Branch_New_ArgumentsIsValid_ReturnTheBranch_Test()
    {
        // Act
        var branch = BranchFixture.Create();

        // Assert
        Assert.Equal(BranchFixture.DefaultAddress, branch.Address);
        Assert.Equal(BranchFixture.DefaultLatitude, branch.Latitude);
        Assert.Equal(BranchFixture.DefaultLongitude, branch.Longitude);
        Assert.Matches(_guidRegex, branch.Guid);
    }

    [Fact]
    public void Branch_New_ArgumentsIsValid_ReturnThe100BranchesWithUniqueGuids_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var branch = BranchFixture.Create();

            // Assert
            Assert.DoesNotContain(branch.Guid, guids);

            guids.Add(branch.Guid);
        }
    }

    [Fact]
    public void
        Branch_CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt_ContextAndArgumentsIsValid_ReturnTheLengthInKmAndDrivingHours_Test()
    {
        // Arrange
        var geolocationServiceStub = GeolocationServiceStub.Create();

        var branchPoint = (BranchFixture.DefaultLatitude, BranchFixture.DefaultLongitude);
        var startPoint = (OrderFixture.DefaultStartPointLatitude, OrderFixture.DefaultStartPointLongitude);
        var endPoint = (OrderFixture.DefaultEndPointLatitude, OrderFixture.DefaultEndPointLongitude);
        var orderCreationRequestDto = OrderFixture.CreateOrderCreationRequestDto();
        var expectedLengthInKmAndDrivingHours =
            geolocationServiceStub.CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(branchPoint,
                startPoint, endPoint);

        var branch = BranchFixture.Create();

        //Act
        var actualLengthInKmAndDrivingHours =
            branch.CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(orderCreationRequestDto,
                geolocationServiceStub);

        // Assert
        Assert.Equal(expectedLengthInKmAndDrivingHours.LengthInKm, actualLengthInKmAndDrivingHours.LengthInKm);
        Assert.Equal(expectedLengthInKmAndDrivingHours.DrivingHours, actualLengthInKmAndDrivingHours.DrivingHours);
    }

    private readonly Regex _guidRegex = RegexFixture.GuidRegex();
}