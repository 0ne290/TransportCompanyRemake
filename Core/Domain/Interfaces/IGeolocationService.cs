namespace Domain.Interfaces;

public interface IGeolocationService
{
    decimal CalculateDistance(decimal latitude, decimal longtitude);
}