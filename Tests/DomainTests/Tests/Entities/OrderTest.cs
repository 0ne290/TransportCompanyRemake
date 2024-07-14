using Domain.Constants;
using Domain.Entities;

namespace DomainTests.Tests.Entities;

public class OrderTest
{
    [Fact]
    public void Order_New_ArgumentsIsValid_ReturnTheOrderWithTwoDriversAndHazardClassFlag()
    {
        var expectedStartAddress = "AnyStartAddress";
        var expectedEndAddress = "AnyEndAddress";
        var expectedDescription = "AnyDescription";
        var expectedStartPointLatitude = 56.9;
        var expectedStartPointLongitude = 4.8;
        var expectedEndPointLatitude = -9;
        var expectedEndPointLongitude = 8;
        var expectedCargoVolume = 60;
        var expectedCargoWeight = 6000;
        var expectedHazardClassFlag = HazardClassesFlags.Class21;
        var expectedTank = true;
        var expectedUser = User.New("AnyName", "AnyContact", 364);
        var expectedBranch = Branch.New("AnyAddress", (34, 75));
        var expectedTruck = Truck.New("AnyNumber", true, 78, 1.2m, 17000, 0.15m, 1,
            HazardClassesFlags.Class21 | HazardClassesFlags.Class22 | HazardClassesFlags.Class23, expectedBranch);
        var expectedDriver1 = Driver.New("AnyDriver1Name", AdrDriverQualificationsFlags.Base, expectedBranch);
        var expectedDriver2 = Driver.New("AnyDriver2Name", AdrDriverQualificationsFlags.Base, expectedBranch);
    }
}