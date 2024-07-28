using System.Globalization;
using System.Security.Cryptography;
using Domain.Constants;
using Domain.Interfaces;

namespace Domain.Entities;

public class User
{
    private User() { }
    
    public static User New(string name, string contact, string login, string password, ICryptographicService cryptographicService)
    {
        var user = new User
        {
            Guid = System.Guid.NewGuid().ToString(), DynamicPartOfSalt = RandomNumberGenerator.GetHexString(128),
            RegistrationDate = DateTime.Now, VkUserId = null, Name = name, Contact = contact
        };
        user.SetLogin(login);
        user.SetPassword(cryptographicService, password);
        
        return user;
    }
    
    public static User New(string name, string contact, long vkUserId)
    {
        var user = new User
        {
            Guid = System.Guid.NewGuid().ToString(), DynamicPartOfSalt = RandomNumberGenerator.GetHexString(128),
            RegistrationDate = DateTime.Now, VkUserId = vkUserId, Login = null, Password = null, Name = name, Contact = contact
        };
        
        return user;
    }

    public Order CreateOrder(string startAddress, string endAddress, string cargoDescription,
        (double Latitude, double Longitude) startPoint, (double Latitude, double Longitude) endPoint,
        decimal cargoVolume, decimal cargoWeight, bool tank, int? hazardClassFlag = null) => Order.New(this,
        startAddress, endAddress, cargoDescription, startPoint, endPoint, cargoVolume, cargoWeight, tank,
        hazardClassFlag);
    
    public async Task<string> GetOrderPaymentUrl(IOrderPaymentService orderPaymentService, Order order)
    {
        if (order.Status != OrderStatuses.PerformersAssigned)
            throw new ArgumentException("Get order payment URL only possible for orders with the \"PerformersAssigned\" status.", nameof(order));
        if (order.UserGuid != Guid)
            throw new ArgumentException("Get order payment URL only possible for user own orders.", nameof(order));
        
        return await orderPaymentService.GetPaymentUrl(order, this);
    }
    
    public void SetLogin(string login)
    {
        if (VkUserId != null)
            throw new InvalidOperationException("VkUserId must be null.");

        Login = login;
    }
    
    public void SetPassword(ICryptographicService cryptographicService, string password)
    {
        if (VkUserId != null)
            throw new InvalidOperationException("VkUserId must be null.");
        
        Password = cryptographicService.EncryptAndHash(Salt(password));
    }

    private string Salt(string value) => value + RegistrationDate.ToString(CultureInfo.InvariantCulture) + StaticPartOfSalt + Login + value + DynamicPartOfSalt + Login;
    
    public override string ToString() => Login == null ? $"VkUserId = {VkUserId}" : $"Login = {Login}";
    
    public string Guid { get; private set; } = null!;
    
    public string DynamicPartOfSalt { get; private set; } = null!;
    
    public DateTime RegistrationDate { get; private set; }

    public long? VkUserId { get; private set; }

    public string? Login { get; private set; }

    public string? Password { get; private set; }
    
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();

    public string Name { get; set; } = null!;

    public string Contact { get; set; } = null!;

    private const string StaticPartOfSalt = "6d9ace9d25bca79be42c971f85a543b22dcee800101d9b39b9213741a5cdcf147b853dc142fa761f66b6cffb50e1a3c5183ae78013124fa58ff41a6edfc6e969";
}