using Entities = Domain.Entities;
using Application.Interfaces;
using Domain.Constants;
using Domain.Interfaces;

namespace Application.Actors;

public class User(IEntityStorageService<Entities.User> userStorageService, IEntityStorageService<Entities.Order> orderStorageService, ICryptographicService cryptographicService)
{
    public async Task<Dtos.User.Response> CreateOrUpdateAndGetVkUser(Dtos.User.CreateVkRequest createRequest)
    {
        try
        {
            var user = await userStorageService.Find(u => u.VkUserId == createRequest.VkUserId);

            if (user.Name != createRequest.Name || user.Contact != createRequest.Contact)
            {
                user.Name = createRequest.Name;
                user.Contact = createRequest.Contact;
                await userStorageService.UpdateRange(new[] { user });
            }

            return new Dtos.User.Response(user.Guid, user.RegistrationDate, user.VkUserId, user.Login, user.Password,
                user.Name, user.Contact, null);
        }
        catch (InvalidOperationException)
        {
            var user = Entities.User.New(createRequest.Name, createRequest.Contact, createRequest.VkUserId);
            await userStorageService.CreateRange(new[] { user });
            
            return new Dtos.User.Response(user.Guid, user.RegistrationDate, user.VkUserId, user.Login, user.Password,
                user.Name, user.Contact, null);
        }
    }

    public async Task<Dtos.User.Response> CreateAndGetStandartUser(Dtos.User.CreateStandartRequest createRequest)
    {
        var user = Entities.User.New(createRequest.Name, createRequest.Contact, createRequest.Login, createRequest.Password, cryptographicService); 
        await userStorageService.CreateRange(new[] { user });

        return new Dtos.User.Response(user.Guid, user.RegistrationDate, user.VkUserId, user.Login, user.Password, user.Name, user.Contact, null);
    }
    
    public async Task<Dtos.User.Response> GetStandartUser(string login, string password)
    {
        var users = await userStorageService.FindAll(_ => true);
        var user = users.First(u =>
            u.Login == login && u.Password == cryptographicService.EncryptAndHash(u.Salt(password)));

        return new Dtos.User.Response(user.Guid, user.RegistrationDate, user.VkUserId, user.Login, user.Password,
            user.Name, user.Contact, null);
    }
    
    /*public async Task CreateStandartUsers(IReadOnlyCollection<Dtos.User.CreateStandartRequest> createRequests)
    {
        var users = createRequests.Select(cr => Entities.User.New(cr.Name, cr.Contact, cr.Login, cr.Password, cryptographicService));

        await userStorageService.CreateRange(users);
    }*/
    
    public async Task CreateOrder(Dtos.Order.CreateRequest createRequest)
    {
        var user = await userStorageService.Find(u => u.Guid == createRequest.UserGuid);

        await orderStorageService.CreateRange(new[]
        {
            Entities.Order.New(user, createRequest.StartAddress, createRequest.EndAddress,
                createRequest.CargoDescription, (createRequest.StartPointLatitude, createRequest.StartPointLongitude),
                (createRequest.EndPointLatitude, createRequest.EndPointLongitude), createRequest.CargoVolume,
                createRequest.CargoWeight, createRequest.TankRequired,
                createRequest.HazardClassFlag == null
                    ? null
                    : HazardClassesFlags.StringToFlagCombination(createRequest.HazardClassFlag))
        });
    }
}