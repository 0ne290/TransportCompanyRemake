namespace Domain.Interfaces;

public interface IGeolocation
{
    decimal CalculateDistance(decimal latitude, decimal longtitude);
}