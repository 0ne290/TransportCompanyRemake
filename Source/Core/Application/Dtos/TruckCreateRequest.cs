namespace Application.Dtos;

public record TruckCreateRequest(string Number, bool Tank, decimal VolumeMax, decimal VolumePrice, decimal WeightMax, decimal WeightPrice, decimal PricePerKm, string BranchGuid, int? PermittedHazardClassesFlags);