namespace Application.Dtos;

public record TruckResponse(string Guid, DateTime CommissionedDate, DateTime? DecommissionedDate, int? PermittedHazardClassesFlags, string Number, bool IsAvailable, bool TrailerIsTank, decimal VolumeMax, decimal VolumePrice, decimal WeightMax, decimal WeightPrice, decimal PricePerKm, BranchResponse? Branch, IEnumerable<OrderResponse>? Orders);