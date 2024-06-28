using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Entities;

public class Branch : IServiceProviderRequired
{
    public override string ToString() => Address;

    public double CalculateDistanceInKilometersByDegrees((double Latitude, double Longitude) point) => ServiceProvider
        .GetRequiredService<IGeolocationService>()
        .CalculateDistanceInKilometersByDegrees((Latitude, Longitude), (point.Latitude, point.Longitude));
    
    public string Address { get; set; } = null!;
    
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public virtual ICollection<Truck> Trucks { get; set; } = new List<Truck>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    
    public IServiceProvider ServiceProvider { get; set; } = DefaultServiceProviderSingleton.Instance;
}