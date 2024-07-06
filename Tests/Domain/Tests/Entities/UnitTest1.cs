using Domain.Entities;

namespace Domain;

public class BranchTest
{
    [Fact]
    public void New_SourceDataAndBranchAutoPropertiesAreEqual_Test()
    {
        var expectedAddress = "AnyAddress";
        var expectedLatitude = 37.314;
        var expectedLongitude = -2.425;

        var branch = Branch.New(expectedAddress, expectedLatitude, expectedLongitude);
        
        Assert.Equal(expectedAddress, branch.Address);
        Assert.Equal(expectedLatitude, branch.Latitude);
        Assert.Equal(expectedLongitude, branch.Longitude);
    }
    
    [Fact]
    public void New_GuidsOfTwoBranchesAreNotEqual_Test()
    {
        var branch1 = Branch.New("Zxc993", 1, 2);
        var branch2 = Branch.New("Zxc993", 1, 2);
        
        Assert.NotEqual(branch1.Guid, branch2.Guid);
    }
}