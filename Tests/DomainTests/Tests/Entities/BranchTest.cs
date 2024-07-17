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
        // Arrange
        const string expectedAddress = "AnyAddress";
        const double expectedLatitude = 37.314;
        const double expectedLongitude = -2.425;

        // Act
        var branch = Branch.New(expectedAddress, (expectedLatitude, expectedLongitude));

        // Assert
        Assert.Equal(expectedAddress, branch.Address);
        Assert.Equal(expectedLatitude, branch.Latitude);
        Assert.Equal(expectedLongitude, branch.Longitude);
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
            var branch = Branch.New("AnyAddress", (37.314, -2.425));

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
        
        var branchPoint = (34.85, -73.64);
        var givenPoint = (22.6, 4);
        var expectedDistance = geolocationServiceStub.CalculateDistanceInKmByDegrees(branchPoint, givenPoint);

        var branch = Branch.New("Anything", branchPoint);

        //Act
        var actualDistance =
            branch.CalculateDistanceInKmByDegrees(geolocationServiceStub, givenPoint);

        // Assert
        Assert.Equal(expectedDistance, actualDistance);
    }
    
    private readonly Regex _guidRegex = RegexFixture.GuidRegex();
}