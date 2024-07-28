using System.Globalization;
using System.Security.Cryptography;
using Domain.Interfaces;

namespace Domain.Entities;

public class Manager
{
    private Manager() { }
    
    public static Manager New(string name, string contact, string login, string password, ICryptographicService cryptographicService)
    {
        var manager = new Manager
        {
            Guid = System.Guid.NewGuid().ToString(), DynamicPartOfSalt = RandomNumberGenerator.GetHexString(128),
            RegistrationDate = DateTime.Now, Name = name, Contact = contact
        };
        manager.SetLogin(login);
        manager.SetPassword(cryptographicService, password);
        
        return manager;
    }

    public void AcceptOrder(Order order) => order.AssignManager(this);
    
    public void AssignPerformersToOrder(Order order, IGeolocationService geolocationService, Truck truck, Driver driver1, Driver? driver2 = null)
    {
        if (order.ManagerGuid != Guid)
            throw new ArgumentException("The manager can assign performers only to his own orders.", nameof(order));
        
        order.AssignPerformers(geolocationService, truck, driver1, driver2);
    }
    
    public void FinishOrder(Order order, double actualHoursWorkedByDriver1, double? actualHoursWorkedByDriver2 = null)
    {
        if (order.ManagerGuid != Guid)
            throw new ArgumentException("The manager can assign performers only to his own orders.", nameof(order));
        
        order.Finish(actualHoursWorkedByDriver1, actualHoursWorkedByDriver2);
    }
    
    public void SetLogin(string login) => Login = login;
    
    public void SetPassword(ICryptographicService cryptographicService, string password) => Password = cryptographicService.EncryptAndHash(Salt(password));

    private string Salt(string value) => value + RegistrationDate.ToString(CultureInfo.InvariantCulture) + StaticPartOfSalt + Login + value + DynamicPartOfSalt + Login;
    
    public override string ToString() => $"Login = {Login}";
    
    public string Guid { get; private set; } = null!;
    
    public string DynamicPartOfSalt { get; private set; } = null!;
    
    public DateTime RegistrationDate { get; private set; }

    public string Login { get; private set; } = null!;

    public string Password { get; private set; } = null!;
    
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();

    public string Name { get; set; } = null!;

    public string Contact { get; set; } = null!;

    private const string StaticPartOfSalt = "9DB898CCAD6F0010345B48A8B346D0FA5E06A8E27800EDA96240A62447713451025988D76440E732F7B87C19E154E30A7EB6832AF29CCBA4934C8AE94299CE21";
}