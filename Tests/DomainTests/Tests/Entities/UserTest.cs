using System.Text.RegularExpressions;
using Domain.Entities;
using Domain.Interfaces;
using DomainTests.Tests.Fixtures;
using DomainTests.Tests.Stubs;

namespace DomainTests.Tests.Entities;

public class UserTest
{
    [Fact]
    public void User_NewStandart_ArgumentsIsValid_ReturnTheUser_Test()
    {
        // Arrange1
        var expextedRegistrationDate = DateTime.Now;
        var expextedRegistrationDateError = TimeSpan.FromSeconds(10);
        
        // Act
        var user = UserFixture.CreateStandart(_cryptographicServiceStub);
        
        // Arrange2
        var expectedPassword = _cryptographicServiceStub.EncryptAndHash(UserFixture.Salt(user, UserFixture.DefaultPassword));

        // Assert
        Assert.Matches(_guidRegex, user.Guid);
        Assert.Matches(_dynamicPartOfSaltRegex, user.DynamicPartOfSalt);
        Assert.Equal(expextedRegistrationDate, user.RegistrationDate, expextedRegistrationDateError);
        Assert.Null(user.VkUserId);
        Assert.NotNull(user.Login);
        Assert.Equal(UserFixture.DefaultLogin, user.Login);
        Assert.NotNull(user.Password);
        Assert.Equal(expectedPassword, user.Password);
        Assert.Equal(UserFixture.DefaultName, user.Name);
        Assert.Equal(UserFixture.DefaultContact, user.Contact);
    }
    
    [Fact]
    public void User_NewStandart_ArgumentsIsValid_ReturnThe100UsersWithUniqueGuidsAndDynamicPartsOfSalt_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var dynamicPartsOfSalt = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var user = UserFixture.CreateStandart(_cryptographicServiceStub);

            // Assert
            Assert.DoesNotContain(user.Guid, guids);
            Assert.DoesNotContain(user.DynamicPartOfSalt, dynamicPartsOfSalt);

            guids.Add(user.Guid);
            dynamicPartsOfSalt.Add(user.DynamicPartOfSalt);
        }
    }
    
    [Fact]
    public void User_NewVk_ArgumentsIsValid_ReturnTheUser_Test()
    {
        // Arrange
        var expextedRegistrationDate = DateTime.Now;
        var expextedRegistrationDateError = TimeSpan.FromSeconds(10);
        
        // Act
        var user = UserFixture.CreateVk();

        // Assert
        Assert.Matches(_guidRegex, user.Guid);
        Assert.Matches(_dynamicPartOfSaltRegex, user.DynamicPartOfSalt);
        Assert.Equal(expextedRegistrationDate, user.RegistrationDate, expextedRegistrationDateError);
        Assert.Null(user.Login);
        Assert.Null(user.Password);
        Assert.NotNull(user.VkUserId);
        Assert.Equal(UserFixture.DefaultVkUserId, user.VkUserId);
        Assert.Equal(UserFixture.DefaultName, user.Name);
        Assert.Equal(UserFixture.DefaultContact, user.Contact);
    }

    [Fact]
    public void User_NewVk_ArgumentsIsValid_ReturnThe100UsersWithUniqueGuidsAndDynamicPartsOfSalt_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var dynamicPartsOfSalt = new HashSet<string>(100);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var user = UserFixture.CreateVk();

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
        var user = UserFixture.CreateStandart(_cryptographicServiceStub, login: "OldLogin");
        
        // Act
        user.SetLogin(expectedLogin);
        
        // Assert
        Assert.Equal(expectedLogin, user.Login);
    }
    
    [Fact]
    public void User_SetLogin_VkUserIdIsInvalid_ThrowTheInvalidOperationException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        
        // Act & Assery
        Assert.Throws<InvalidOperationException>(() => user.SetLogin("NewLogin"));
    }
    
    [Fact]
    public void User_SetPassword_ContextAndArgumentIsValid_SetThePassword()
    {
        // Arrange
        const string newPassword = "NewPassword";
        var user = UserFixture.CreateStandart(_cryptographicServiceStub, password: "OldPassword");
        var expectedPassword = _cryptographicServiceStub.EncryptAndHash(UserFixture.Salt(user, newPassword));
        
        // Act
        user.SetPassword(_cryptographicServiceStub, newPassword);
        
        // Assert
        Assert.Equal(expectedPassword, user.Password);
    }
    
    [Fact]
    public void User_SetPassword_VkUserIdIsInvalid_ThrowTheInvalidOperationException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        
        // Act & Assery
        Assert.Throws<InvalidOperationException>(() => user.SetPassword(_cryptographicServiceStub, "NewPassword"));
    }

    private readonly ICryptographicService _cryptographicServiceStub = CryptographicServiceStub.Create();
    
    private readonly Regex _guidRegex = RegexFixture.GuidRegex();

    private readonly Regex _dynamicPartOfSaltRegex = RegexFixture.DynamicPartOfSaltRegex();
}