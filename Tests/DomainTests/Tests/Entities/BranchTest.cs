using System.Text.RegularExpressions;
using Domain.Entities;
using DomainTests.Tests.Fixtures;
using DomainTests.Tests.Stubs;

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
        Branch_CalculateDistanceInKmByDegrees_ContextAndArgumentsIsValid_ReturnTheDistanceFromBranchPointToGivenPoint_Test()
    {
        // Arrange
        var geolocationServiceStub = GeolocationServiceStub.Create();
        
        var branchPoint = (BranchFixture.DefaultLatitude, BranchFixture.DefaultLongitude);
        var givenPoint = (22.6, 4);
        var expectedDistance = geolocationServiceStub.CalculateDistanceInKmByDegrees(branchPoint, givenPoint);

        var branch = BranchFixture.Create();

        //Act
        var actualDistance =
            branch.CalculateDistanceInKmByDegrees(geolocationServiceStub, givenPoint);

        // Assert
        Assert.Equal(expectedDistance, actualDistance);
    }
    
    private readonly Regex _guidRegex = RegexFixture.GuidRegex();
}