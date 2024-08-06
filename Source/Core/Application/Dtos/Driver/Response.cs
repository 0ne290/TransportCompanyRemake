namespace Application.Dtos;

public record Response(string Guid, DateTime HireDate, DateTime? DismissalDate, double HoursWorkedPerWeek, double TotalHoursWorked, int? AdrQualificationFlag, bool AdrQualificationOfTank, string Name, bool IsAvailable, BranchResponse? Branch, IEnumerable<OrderResponse>? Orders);