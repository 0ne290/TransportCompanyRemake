using Domain.Services;

namespace DomainTests.Tests.Services;

public class DefaultGeolocationServiceTest
{
    [Fact]
    public void CalculateDistanceInKmByDegrees_ReturnTheDistanceCalculatedUsingHaversineFormulaModifiedForAntipodes_Test()
    {
        // Arrange
        var point1 = (52, 103);
        var point2 = (52, 104);
        const double expectedDistance = 68.458;
        var defaultGeolocationService = new DefaultGeolocationService();

        // Act
        var actualDistance = defaultGeolocationService.CalculateDistanceInKmByDegrees(point1, point2);
        
        // Assert
        Assert.Equal(expectedDistance, actualDistance, 3);
    }
}