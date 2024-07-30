namespace Application.Dtos;

public record BranchResponse(string Guid, string Address, double Latitude, double Longitude, IEnumerable<TruckResponse> Trucks, IEnumerable<DriverResponse> Drivers);