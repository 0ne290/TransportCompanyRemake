namespace Domain.Interfaces;

public interface IServiceProviderRequired
{
    IServiceProvider ServiceProvider { get; set; }
}