using Entities = Domain.Entities;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Actors;

public class User(IEntityStorageService<Entities.User> userStorageService, ICryptographicService cryptographicService)
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
        var user = await userStorageService.Find(u => u.Login == login && u.Password == password);

        return new Dtos.User.Response(user.Guid, user.RegistrationDate, user.VkUserId, user.Login, user.Password,
            user.Name, user.Contact, null);
    }
    
    /*public async Task CreateStandartUsers(IReadOnlyCollection<Dtos.User.CreateStandartRequest> createRequests)
    {
        var users = createRequests.Select(cr => Entities.User.New(cr.Name, cr.Contact, cr.Login, cr.Password, cryptographicService));

        await userStorageService.CreateRange(users);
    }
    
    public void CreateOrder()
    {
        
    }*/
}