using Domain.Entities;

namespace DomainTests.Fixtures;

public static class BranchFixture
{
    public static Branch Create(string address = DefaultAddress, double latitude = DefaultLatitude,
        double longitude = DefaultLongitude) => Branch.New(address, (latitude, longitude));
    
    public const string DefaultAddress = "AnyAddress";
    
    public const double DefaultLatitude = 34;
    
    public const double DefaultLongitude = 75;
}