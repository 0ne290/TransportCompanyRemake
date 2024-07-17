using System.Globalization;
using System.Text.RegularExpressions;
using Domain.Entities;
using Domain.Interfaces;
using DomainTests.Tests.Fixtures;
using DomainTests.Tests.Stubs;

namespace DomainTests.Tests.Entities;

public class UserTest
{
    [Fact]
    public void User_New_ArgumentsIsValid_ReturnTheDefaultUser_Test()
    {
        // Arrange1
        const string expectedName = "AnyName";
        const string expectedContact = "AnyContact";
        const string expectedLogin = "AnyLogin";
        const string password = "AnyPassword";

        var expextedRegistrationDate = DateTime.Now;
        var expextedRegistrationDateError = TimeSpan.FromSeconds(10);
        
        // Act
        var user = User.New(expectedName, expectedContact, expectedLogin, password, _cryptographicServiceStub);
        
        // Arrange2
        var expectedPassword = _cryptographicServiceStub.EncryptAndHash(ExpectedSalt(password, user));

        // Assert
        Assert.Matches(_guidRegex, user.Guid);
        Assert.Matches(_dynamicPartOfSaltRegex, user.DynamicPartOfSalt);
        Assert.Equal(expextedRegistrationDate, user.RegistrationDate, expextedRegistrationDateError);
        Assert.Null(user.VkUserId);
        Assert.NotNull(user.Login);
        Assert.Equal(expectedLogin, user.Login);
        Assert.NotNull(user.Password);
        Assert.Equal(expectedPassword, user.Password);
        Assert.Equal(expectedName, user.Name);
        Assert.Equal(expectedContact, user.Contact);
    }
    
    [Fact]
    public void User_New_ArgumentsIsValid_ReturnTheVkUser_Test()
    {
        // Arrange1
        const string expectedName = "AnyName";
        const string expectedContact = "AnyContact";
        const long expectedVkUserId = 26535;
        
        var expextedRegistrationDate = DateTime.Now;
        var expextedRegistrationDateError = TimeSpan.FromSeconds(10);
        
        // Act
        var user = User.New(expectedName, expectedContact, expectedVkUserId);

        // Assert
        Assert.Matches(_guidRegex, user.Guid);
        Assert.Matches(_dynamicPartOfSaltRegex, user.DynamicPartOfSalt);
        Assert.Equal(expextedRegistrationDate, user.RegistrationDate, expextedRegistrationDateError);
        Assert.Null(user.Login);
        Assert.Null(user.Password);
        Assert.NotNull(user.VkUserId);
        Assert.Equal(expectedVkUserId, user.VkUserId);
        Assert.Equal(expectedName, user.Name);
        Assert.Equal(expectedContact, user.Contact);
    }

    [Fact]
    public void User_NewTheVkUser_ArgumentsIsValid_ReturnThe100UsersWithUniqueGuidsAndDynamicPartsOfSalt_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var dynamicPartsOfSalt = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var user = User.New("AnyName", "AnyContact", 35);

            // Assert
            Assert.DoesNotContain(user.Guid, guids);
            Assert.DoesNotContain(user.DynamicPartOfSalt, dynamicPartsOfSalt);

            guids.Add(user.Guid);
            dynamicPartsOfSalt.Add(user.DynamicPartOfSalt);
        }
    }
    
    [Fact]
    public void User_NewTheDefaultUser_ArgumentsIsValid_ReturnThe100UsersWithUniqueGuidsAndDynamicPartsOfSalt_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var dynamicPartsOfSalt = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var user = User.New("AnyName", "AnyContact", "AnyLogin", "AnyPassword", _cryptographicServiceStub);

            // Assert
            Assert.DoesNotContain(user.Guid, guids);
            Assert.DoesNotContain(user.DynamicPartOfSalt, dynamicPartsOfSalt);

            guids.Add(user.Guid);
            dynamicPartsOfSalt.Add(user.DynamicPartOfSalt);
        }
    }

    [Fact]
    public void User_SetLogin_ContextAndArgumentIsValid_SetTheLogin()
    {
        // Arrange
        const string expectedLogin = "NewLogin";
        
        var user = User.New("AnyName", "AnyContact", "AnyLogin", "AnyPassword", _cryptographicServiceStub);
        
        // Act
        user.SetLogin(expectedLogin);
        
        // Assert
        Assert.Equal(expectedLogin, user.Login);
    }
    
    [Fact]
    public void User_SetLogin_VkUserIdIsInvalid_ThrowTheInvalidOperationException()
    {
        // Arrange
        var user = User.New("AnyName", "AnyContact", 365);
        
        // Act & Assery
        Assert.Throws<InvalidOperationException>(() => user.SetLogin("NewLogin"));
    }
    
    [Fact]
    public void User_SetPassword_ContextAndArgumentIsValid_SetThePassword()
    {
        // Arrange
        const string newPassword = "NewPassword";
        
        var user = User.New("AnyName", "AnyContact", "AnyLogin", "AnyPassword", _cryptographicServiceStub);
        
        var expectedPassword = _cryptographicServiceStub.EncryptAndHash(ExpectedSalt(newPassword, user));
        
        // Act
        user.SetPassword(_cryptographicServiceStub, newPassword);
        
        // Assert
        Assert.Equal(expectedPassword, user.Password);
    }
    
    [Fact]
    public void User_SetPassword_VkUserIdIsInvalid_ThrowTheInvalidOperationException()
    {
        // Arrange
        var user = User.New("AnyName", "AnyContact", 365);
        
        // Act & Assery
        Assert.Throws<InvalidOperationException>(() => user.SetPassword(_cryptographicServiceStub, "NewPassword"));
    }
    
    private static string ExpectedSalt(string value, User user)
    {
        const string staticPartOfSalt = "6d9ace9d25bca79be42c971f85a543b22dcee800101d9b39b9213741a5cdcf147b853dc142fa761f66b6cffb50e1a3c5183ae78013124fa58ff41a6edfc6e969";
        
        return value + user.RegistrationDate.ToString(CultureInfo.InvariantCulture) + staticPartOfSalt + user.Login + value + user.DynamicPartOfSalt +
            user.Login;
    }

    private readonly ICryptographicService _cryptographicServiceStub = CryptographicServiceStub.Create();
    
    private readonly Regex _guidRegex = RegexFixture.GuidRegex();

    private readonly Regex _dynamicPartOfSaltRegex = RegexFixture.DynamicPartOfSaltRegex();
}