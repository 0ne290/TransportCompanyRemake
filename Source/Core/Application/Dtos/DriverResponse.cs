namespace Application.Dtos;

public record DriverResponse(string Guid, DateTime HireDate, DateTime? DismissalDate, double HoursWorkedPerWeek, double TotalHoursWorked, int? AdrQualificationFlag, bool AdrQualificationOfTank, string BranchGuid, string Name, bool IsAvailable, BranchResponse Branch, IEnumerable<OrderResponse> Orders);