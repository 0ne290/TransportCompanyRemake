using Entities = Domain.Entities;
using Application.Interfaces;

namespace Application.Actors;

public class User(IEntityStorageService<Entities.User> userStorageService)
{
    public async Task<Dtos.User.Response> TryCreateVkUser(Dtos.User.CreateVkRequest createRequest)
    {
        Entities.User? user = await userStorageService.Find(u => u.VkUserId == createRequest.VkUserId);
        if (user == null)
        {
            user = Entities.User.New(createRequest.Name, createRequest.Contact, createRequest.VkUserId);
            await userStorageService.CreateRange(new[] { user });
        }
        else if (user.Name != createRequest.Name || user.Contact != createRequest.Contact)
        {
            user.Name = createRequest.Name;
            user.Contact = createRequest.Contact;
            await userStorageService.UpdateRange(new[] { user });
        }

        return new Dtos.User.Response(user.Guid, user.RegistrationDate, user.VkUserId, user.Login, user.Password, user.Name, user.Contact, null);
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