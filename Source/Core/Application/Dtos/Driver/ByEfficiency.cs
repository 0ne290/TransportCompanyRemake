namespace Application.Dtos.Driver;

public record ByEfficiency(string DriverName, decimal PriceOfAllOrders, double HoursWorked, decimal PriceOfAllOrdersPerHour);