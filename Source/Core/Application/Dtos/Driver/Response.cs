using Application.Dtos.Branch;
using Application.Dtos.Order;

namespace Application.Dtos.Driver;

public record Response(string Guid, DateTime HireDate, DateTime? DismissalDate, double HoursWorkedPerWeek, double TotalHoursWorked, string? AdrQualificationFlag, bool AdrQualificationOfTank, string Name, bool IsAvailable, Branch.Response? Branch, IEnumerable<Order.Response>? PrimaryOrders, IEnumerable<Order.Response>? SecondaryOrders);