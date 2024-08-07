using Application.Dtos.Branch;
using Application.Dtos.Order;
using Response = Application.Dtos.Order.Response;

namespace Application.Dtos.Truck;

public record Response(string Guid, DateTime CommissionedDate, DateTime? DecommissionedDate, int? PermittedHazardClassesFlags, string Number, bool IsAvailable, bool TrailerIsTank, decimal VolumeMax, decimal VolumePrice, decimal WeightMax, decimal WeightPrice, decimal PricePerKm, Branch.Response? Branch, IEnumerable<Order.Response>? Orders);