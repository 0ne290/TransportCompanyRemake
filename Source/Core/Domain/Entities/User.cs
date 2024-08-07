using System.Globalization;
using System.Security.Cryptography;
using Domain.Interfaces;

namespace Domain.Entities;

public class User
{
    private User() { }
    
    public static User New(string name, string contact, string login, string password, ICryptographicService cryptographicService)
    {
        var user = new User { Name = name, Contact = contact, Login = login };
        user.Password = cryptographicService.EncryptAndHash(user.Salt(password));
        
        return user;
    }

    public static User New(string name, string contact, long vkUserId) =>
        new() { Name = name, Contact = contact, VkUserId = vkUserId };
    
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

    public string Guid { get; private set; } = System.Guid.NewGuid().ToString();

    public string DynamicPartOfSalt { get; private set; } = RandomNumberGenerator.GetHexString(128);

    public DateTime RegistrationDate { get; private set; } = DateTime.Now;

    public long? VkUserId { get; private set; }

    public string? Login { get; private set; }

    public string? Password { get; private set; }

    public string Name { get; set; } = null!;

    public string Contact { get; set; } = null!;
    
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();

    private const string StaticPartOfSalt = "6d9ace9d25bca79be42c971f85a543b22dcee800101d9b39b9213741a5cdcf147b853dc142fa761f66b6cffb50e1a3c5183ae78013124fa58ff41a6edfc6e969";
}