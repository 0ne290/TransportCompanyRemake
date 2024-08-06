using Application.Dtos.Branch;
using Application.Dtos.Order;

namespace Application.Dtos.Driver;

public record Response(string Guid, DateTime HireDate, DateTime? DismissalDate, double HoursWorkedPerWeek, double TotalHoursWorked, int? AdrQualificationFlag, bool AdrQualificationOfTank, string Name, bool IsAvailable, Branch.Response? Branch, IEnumerable<Order.Response>? Orders);