using System.Globalization;
using Domain.Entities;
using Domain.Interfaces;

namespace DomainTests.Tests.Fixtures;

public static class UserFixture
{
    public static User CreateStandart(ICryptographicService cryptographicService, string name = DefaultName, string contact = DefaultContact,
        string login = DefaultLogin, string password = DefaultPassword) => User.New(name, contact, login, password, cryptographicService);
    
    public static User CreateVk(string name = DefaultName, string contact = DefaultContact,
        long vkUserId = DefaultVkUserId) => User.New(name, contact, vkUserId);

    public static string Salt(User user, string value) => value +
                                                          user.RegistrationDate.ToString(CultureInfo.InvariantCulture) +
                                                          StaticPartOfSalt + user.Login + value +
                                                          user.DynamicPartOfSalt + user.Login;
    
    public const string DefaultName = "AnyName";
    
    public const string DefaultContact = "AnyContact";

    public const string DefaultLogin = "AnyLogin";

    public const string DefaultPassword = "AnyPassword";
    
    public const long DefaultVkUserId = 364;

    private const string StaticPartOfSalt =
        "6d9ace9d25bca79be42c971f85a543b22dcee800101d9b39b9213741a5cdcf147b853dc142fa761f66b6cffb50e1a3c5183ae78013124fa58ff41a6edfc6e969";
}