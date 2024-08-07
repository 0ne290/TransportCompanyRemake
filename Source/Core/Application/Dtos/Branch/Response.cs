using Application.Dtos.Driver;
using Application.Dtos.Truck;

namespace Application.Dtos.Branch;

public record Response(string Guid, string Address, double Latitude, double Longitude, IEnumerable<Truck.Response>? Trucks, IEnumerable<Driver.Response>? Drivers);