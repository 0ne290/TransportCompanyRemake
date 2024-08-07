using Application.Dtos.Branch;
using Application.Dtos.Driver;
using Application.Dtos.Truck;
using Application.Dtos.User;
using Response = Application.Dtos.Branch.Response;

namespace Application.Dtos.Order;

public record Response(string Guid, string Status, DateTime DateCreated, DateTime? DateAssignmentOfPerformers, DateTime? DatePaymentAndBegin, DateTime? DateEnd, int? HazardClassFlag, bool Tank, double? LengthInKm, decimal? Price, double? ExpectedHoursWorkedByDrivers, double? ActualHoursWorkedByDriver1, double? ActualHoursWorkedByDriver2, User.Response? User, Truck.Response? Truck, Driver.Response? Driver1, Driver.Response? Driver2, Branch.Response? Branch, string StartAddress, string EndAddress, string CargoDescription, double StartPointLatitude, double StartPointLongitude, double EndPointLatitude, double EndPointLongitude, decimal CargoVolume, decimal CargoWeight);