using System.Globalization;
using Domain.Constants;

namespace Web.Dtos;

public record Order
{
    public Order(Application.Dtos.Order.Response order)
    {
        Guid = order.Guid;
        Status = order.Status switch
        {
            OrderStatuses.AwaitingAssignmentOfPerformers => "Ожидает назначения",
            OrderStatuses.PerformersAssigned => "Ожидает оплаты",
            OrderStatuses.InProgress => "Выполняется",
            OrderStatuses.Completed => "Завершен",
            _ => throw new ArgumentException("Order.Status is invalid.", nameof(order))
        };
        DateCreated = order.DateCreated.ToString(CultureInfo.InvariantCulture);
        DateAssignmentOfPerformers = order.DateAssignmentOfPerformers?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        DatePaymentAndBegin = order.DatePaymentAndBegin?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        DateEnd = order.DateEnd?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        HazardClassFlag = order.HazardClassFlag ?? "Нет";
        TankRequired = order.TankRequired ? "Цистерна" : "Тент";
        LengthInKm = order.LengthInKm?.ToString("F6", CultureInfo.InvariantCulture) ?? string.Empty;
        Price = order.Price?.ToString("F6", CultureInfo.InvariantCulture) ?? string.Empty;
        ExpectedHoursWorkedByDrivers =
            order.ExpectedHoursWorkedByDrivers?.ToString("F6", CultureInfo.InvariantCulture) ?? string.Empty;
        ActualHoursWorkedByDriver1 = order.ActualHoursWorkedByDriver1?.ToString("F6", CultureInfo.InvariantCulture) ?? string.Empty;
        ActualHoursWorkedByDriver2 = order.ActualHoursWorkedByDriver2?.ToString("F6", CultureInfo.InvariantCulture) ?? string.Empty;
        UserName = order.User?.Name ?? string.Empty;
        TruckNumber = order.Truck?.Number ?? string.Empty;
        Driver1Name = order.Driver1?.Name ?? string.Empty;
        Driver2Name = order.Driver2?.Name ?? string.Empty;
        BranchAddress = order.Branch?.Address ?? string.Empty;
        StartAddress = order.StartAddress;
        EndAddress = order.EndAddress;
        CargoDescription = order.CargoDescription;
        StartPointLatitude = order.StartPointLatitude.ToString("F6", CultureInfo.InvariantCulture);
        StartPointLongitude = order.StartPointLongitude.ToString("F6", CultureInfo.InvariantCulture);
        EndPointLatitude = order.EndPointLatitude.ToString("F6", CultureInfo.InvariantCulture);
        EndPointLongitude = order.EndPointLongitude.ToString("F6", CultureInfo.InvariantCulture);
        CargoVolume = order.CargoVolume.ToString("F6", CultureInfo.InvariantCulture);
        CargoWeight = order.CargoWeight.ToString("F6", CultureInfo.InvariantCulture);
    }
    
    public string Guid { get; }

    public string Status { get; }

    public string DateCreated { get; }

    public string DateAssignmentOfPerformers { get; }

    public string DatePaymentAndBegin { get; }

    public string DateEnd { get; }

    public string HazardClassFlag { get; }

    public string TankRequired { get; }

    public string LengthInKm { get; }

    public string Price { get; }

    public string ExpectedHoursWorkedByDrivers { get; }

    public string ActualHoursWorkedByDriver1 { get; }

    public string ActualHoursWorkedByDriver2 { get; }

    public string UserName { get; }

    public string TruckNumber { get; }

    public string Driver1Name { get; }

    public string Driver2Name { get; }

    public string BranchAddress { get; }

    public string StartAddress { get; }

    public string EndAddress { get; }

    public string CargoDescription { get; }

    public string StartPointLatitude { get; }

    public string StartPointLongitude { get; }

    public string EndPointLatitude { get; }

    public string EndPointLongitude { get; }

    public string CargoVolume { get; }

    public string CargoWeight { get; }
}