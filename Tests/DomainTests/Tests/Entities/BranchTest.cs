using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace DomainTests.Tests.Entities;

public class BranchTest
{
    [Fact]
    public void New_SourceDataAndBranchAutoPropertiesAreEqual_Test()
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
    }
    
    [Fact]
    public void New_GuidsOfTwoBranchesAreNotEqual_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        
        for (var i = 0; i < 100; i++)
        {
            // Act
            var branch = Branch.New("Zxc993", (1, 2));
            
            // Assert
            Assert.DoesNotContain(branch.Guid, guids);

            guids.Add(branch.Guid);
        }
    }
    
    [Fact]
    public void CalculateDistanceInKmByDegrees_ReturnTheDistanceFromBranchPointToGivenPoint_Test()
    {
        // Arrange
        var branchPoint = (34.85, -73.64);
        var givenPoint = (22.6, 4);
        var expectedDistance =
            StubOfCalculateDistanceInKmByDegrees(branchPoint, givenPoint);
        
        var mock = new Mock<IGeolocationService>();
        mock.Setup(gs =>
            gs.CalculateDistanceInKmByDegrees(It.IsAny<ValueTuple<double, double>>(),
                It.IsAny<ValueTuple<double, double>>())).Returns(StubOfCalculateDistanceInKmByDegrees);
        
        var stubOfgeolocationService = mock.Object;

        var branch = Branch.New("Anything", branchPoint);
        
        //Act
        var actualDistance =
            branch.CalculateDistanceInKmByDegrees(stubOfgeolocationService, givenPoint);
        
        // Assert
        Assert.Equal(expectedDistance, actualDistance);
        return;
        
        double StubOfCalculateDistanceInKmByDegrees((double Latitude, double Longitude) point1,
            (double Latitude, double Longitude) point2) =>
            point1.Latitude + point1.Longitude + point2.Latitude + point2.Longitude;
    }
}