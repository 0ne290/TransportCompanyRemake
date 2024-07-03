using System.Security.Cryptography;
using System.Text;
using Domain.Interfaces;

namespace Domain.Entities;

public class User
{
    private User() { }
    
    public static User New(string login, string password, string name, string contact) => new() { Guid = System.Guid.NewGuid().ToString(), DynamicPartOfSalt = RandomNumberGenerator.GetHexString(128), RegistrationDate = DateTime.Now }

    public void SetVkUserId(long? vkUserId)
    {
        if (vkUserId == null && (Login == null || Password == null))
            throw new ArgumentOutOfRangeException(nameof(vkUserId), vkUserId,
                "The VkUserId value can be null only when the Login and Password values are not null.");
        if (vkUserId != null && (Login != null || Password != null))
            throw new ArgumentOutOfRangeException(nameof(vkUserId), vkUserId,
                "The VkUserId value cannot be null only when the login and password values are null.");

        VkUserId = vkUserId;
    }
    
    public void SetLogin(string? login)
    {
        if (login == null && (Password != null || VkUserId == null))
            throw new ArgumentOutOfRangeException(nameof(login), login,
                "The Login login can be null only when the Password login are null and VkUserId login are not null.");
        if (login != null && (Password == null || VkUserId != null))
            throw new ArgumentOutOfRangeException(nameof(login), login,
                "The Login login cannot be null only when the Password login are not null and VkUserId login are null.");

        Login = login;
    }
    
    public void SetPassword(ICryptographicService cryptographicService, string? password)
    {
        if (password == null && (Login != null || VkUserId == null))
            throw new ArgumentOutOfRangeException(nameof(password), password,
                "The Password passwod can be null only when the Login passwod are null and VkUserId passwod are not null.");
        if (password != null && (Login == null || VkUserId != null))
            throw new ArgumentOutOfRangeException(nameof(password), password,
                "The Password passwod cannot be null only when the Login passwod are not null and VkUserId passwod are null.");

        Password = password == null ? password : cryptographicService.Hash(Salt(password));
    }
    
    public void SetLoginAndPassword(ICryptographicService cryptographicService, string? login, string? password)
    {
        if (login == null && (password != null || VkUserId == null))
            throw new ArgumentOutOfRangeException(nameof(login), login,
                "The Login value can be null only when the Password value are null and VkUserId value are not null.");
        if (password != null && (login == null || VkUserId != null))
            throw new ArgumentOutOfRangeException(nameof(password), password,
                "The Password value cannot be null only when the Login value are not null and VkUserId value are null.");

        Login = login;
        Password = password == null ? password : cryptographicService.Hash(Salt(password));
    }

    private string Salt(string value) => value + StaticPartOfSalt + Login + value + DynamicPartOfSalt + Login;
    
    public override string ToString() => (Login ?? VkUserId.ToString())!;
    
    public string Guid { get; private set; } = null!;
    
    public string DynamicPartOfSalt { get; private set; } = null!;
    
    public DateTime RegistrationDate { get; private set; }

    public long? VkUserId { get; private set; }

    public string? Login { get; private set; }

    public string? Password { get; private set; }

    public string Name { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();

    private const string StaticPartOfSalt = "6d9ace9d25bca79be42c971f85a543b22dcee800101d9b39b9213741a5cdcf147b853dc142fa761f66b6cffb50e1a3c5183ae78013124fa58ff41a6edfc6e969";
}