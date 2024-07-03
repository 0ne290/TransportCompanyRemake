using Domain.Interfaces;

namespace Domain.Entities;

public class Branch
{
    private Branch() { }

    public static Branch New(string address, double latitude, double longitude) => new()
        { Guid = System.Guid.NewGuid().ToString(), Address = address, Latitude = latitude, Longitude = longitude };
    
    public double CalculateDistanceInKmByDegrees(IGeolocationService geolocationService,
        (double Latitude, double Longitude) point) =>
        geolocationService.CalculateDistanceInKmByDegrees((Latitude, Longitude), (point.Latitude, point.Longitude));
    
    public override string ToString() => Address;
    
    public string Guid { get; private set; } = null!;
    
    public string Address { get; set; } = null!;
    
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public virtual ICollection<Truck> Trucks { get; private set; } = new List<Truck>();

    public virtual ICollection<Driver> Drivers { get; private set; } = new List<Driver>();
}