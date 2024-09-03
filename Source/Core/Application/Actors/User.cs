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

    public async Task<IEnumerable<Dtos.Order.Response>> GetOrders(string userGuid) =>
        (await userStorageService.Find(u => u.Guid == userGuid, "Orders")).Orders.Select(o =>
            new Dtos.Order.Response(o.Guid, o.Status, o.DateCreated, o.DateAssignmentOfPerformers,
                o.DatePaymentAndBegin, o.DateEnd,
                o.HazardClassFlag == null ? null : HazardClassesFlags.FlagCombinationToString(o.HazardClassFlag.Value),
                o.TankRequired, o.LengthInKm, o.Price, o.ExpectedHoursWorkedByDrivers, o.ActualHoursWorkedByDriver1,
                o.ActualHoursWorkedByDriver2,
                new Dtos.User.Response(o.User.Guid, o.User.RegistrationDate, o.User.VkUserId, o.User.Login,
                    o.User.Password, o.User.Name, o.User.Contact, null),
                o.Truck == null
                    ? null
                    : new Dtos.Truck.Response(o.Truck.Guid, o.Truck.CommissionedDate, o.Truck.DecommissionedDate,
                        o.Truck.PermittedHazardClassesFlags == null
                            ? null
                            : HazardClassesFlags.FlagCombinationToString(o.Truck.PermittedHazardClassesFlags.Value),
                        o.Truck.Number, o.Truck.IsAvailable, o.Truck.TrailerIsTank, o.Truck.VolumeMax,
                        o.Truck.VolumePrice, o.Truck.WeightMax, o.Truck.WeightPrice, o.Truck.PricePerKm, null, null,
                        null),
                o.Driver1 == null
                    ? null
                    : new Dtos.Driver.Response(o.Driver1.Guid, o.Driver1.HireDate, o.Driver1.DismissalDate,
                        o.Driver1.HoursWorkedPerWeek, o.Driver1.TotalHoursWorked,
                        o.Driver1.AdrQualificationFlag == null
                            ? null
                            : AdrDriverQualificationsFlags.FlagToString(o.Driver1.AdrQualificationFlag.Value),
                        o.Driver1.AdrQualificationOfTank, o.Driver1.Name, o.Driver1.IsAvailable, null, null, null),
                o.Driver2 == null
                    ? null
                    : new Dtos.Driver.Response(o.Driver2.Guid, o.Driver2.HireDate, o.Driver2.DismissalDate,
                        o.Driver2.HoursWorkedPerWeek, o.Driver2.TotalHoursWorked,
                        o.Driver2.AdrQualificationFlag == null
                            ? null
                            : AdrDriverQualificationsFlags.FlagToString(o.Driver2.AdrQualificationFlag.Value),
                        o.Driver2.AdrQualificationOfTank, o.Driver2.Name, o.Driver2.IsAvailable, null, null, null),
                o.Branch == null
                    ? null
                    : new Dtos.Branch.Response(o.Branch.Guid, o.Branch.Address, o.Branch.Latitude, o.Branch.Longitude,
                        null, null, null, null), o.StartAddress, o.EndAddress, o.CargoDescription, o.StartPointLatitude,
                o.StartPointLongitude, o.EndPointLatitude, o.EndPointLongitude, o.CargoVolume, o.CargoWeight));
}