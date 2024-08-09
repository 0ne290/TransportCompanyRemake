using Entities = Domain.Entities;
using System.Linq.Expressions;
using Application.Interfaces;
using Domain.Constants;
using Domain.Interfaces;

namespace Application.Actors;

// TODO: Упростить получение связанных данных: заменить все булевы аргументы одним строковым аргументом, описывающим подтягиваемые связанные свойства
// TODO: Заменить тип Expression<Func<TEntity, bool>> аргументов фильтрации на string - эти строки должны парситься на выражения в слое Application, а не в UI
// TODO: Мой перфекционизм сыграл со мной злую шутку - пытаясь сделать универсальный супер-API, я бесмыссленно и неоправданно его переусложнил, тем самым выстрелив себе в ногу С ДРОБОВИКА %!?*№! В рамках Use Case'ов не будет задействована львиная доля системы. Этот комментарий останется, чтобы я всегда помнил эту ошибку и больше ее никогда не повторил
public class Administrator(IEntityStorageService<Entities.Driver> driverStorageService, IEntityStorageService<Entities.Truck> truckStorageService, IEntityStorageService<Entities.User> userStorageService, IEntityStorageService<Entities.Branch> branchStorageService, IEntityStorageService<Entities.Order> orderStorageService, ICryptographicService cryptographicService, IGeolocationService geolocationService)
{
    public async Task CreateDrivers(IReadOnlyCollection<Dtos.Driver.CreateRequest> createRequests)
    {
        var branchGuids = new HashSet<string>(createRequests.Count);
        foreach (var createRequest in createRequests)
            branchGuids.Add(createRequest.BranchGuid);
        var branches = (await branchStorageService.FindAll(b => branchGuids.Contains(b.Guid))).ToDictionary(b => b.Guid);

        var drivers = new List<Entities.Driver>(createRequests.Count);
        foreach (var createRequest in createRequests)
        {
            if (!branches.TryGetValue(createRequest.BranchGuid, out var branch))
                throw new ArgumentException($"The branch {createRequest.BranchGuid} does not exist.",
                    nameof(createRequests));
            
            drivers.Add(Entities.Driver.New(createRequest.Name, branch, createRequest.AdrQualificationFlag == null ? null : AdrDriverQualificationsFlags.StringToFlag(createRequest.AdrQualificationFlag), createRequest.AdrQualificationOfTank));
        }
        
        await driverStorageService.CreateRange(drivers);
    }

    public async Task<IEnumerable<Dtos.Driver.Response>> GetDrivers(Expression<Func<Entities.Driver, bool>> filter, bool includeBranch, bool includeOrders)
    {
        var includedData = "";
        Func<Entities.Driver, Dtos.Branch.Response?> getBranchResponse;
        if (includeBranch)
        {
            includedData += "Branch;";
            getBranchResponse = d =>
                new Dtos.Branch.Response(d.Branch.Guid, d.Branch.Address, d.Branch.Latitude, d.Branch.Longitude, null,
                    null, null, null);
        }
        else
            getBranchResponse = _ => null;
        
        Func<Entities.Driver, IEnumerable<Dtos.Order.Response>?> getPrimaryOrderResponses;
        Func<Entities.Driver, IEnumerable<Dtos.Order.Response>?> getSecondaryOrderResponses;
        if (includeOrders)
        {
            includedData += "PrimaryOrders;SecondaryOrders;";
            getPrimaryOrderResponses = d => d.PrimaryOrders.Select(o => new Dtos.Order.Response(o.Guid, o.Status, o.DateCreated,
                o.DateAssignmentOfPerformers, o.DatePaymentAndBegin, o.DateEnd, o.HazardClassFlag == null ? null : HazardClassesFlags.FlagCombinationToString(o.HazardClassFlag.Value), o.TankRequired, o.LengthInKm,
                o.Price, o.ExpectedHoursWorkedByDrivers, o.ActualHoursWorkedByDriver1, o.ActualHoursWorkedByDriver2,
                null, null, null, null, null, o.StartAddress, o.EndAddress, o.CargoDescription, o.StartPointLatitude,
                o.StartPointLongitude, o.EndPointLatitude, o.EndPointLongitude, o.CargoVolume, o.CargoWeight));
            getSecondaryOrderResponses = d => d.SecondaryOrders.Select(o => new Dtos.Order.Response(o.Guid, o.Status, o.DateCreated,
                o.DateAssignmentOfPerformers, o.DatePaymentAndBegin, o.DateEnd, o.HazardClassFlag == null ? null : HazardClassesFlags.FlagCombinationToString(o.HazardClassFlag.Value), o.TankRequired, o.LengthInKm,
                o.Price, o.ExpectedHoursWorkedByDrivers, o.ActualHoursWorkedByDriver1, o.ActualHoursWorkedByDriver2,
                null, null, null, null, null, o.StartAddress, o.EndAddress, o.CargoDescription, o.StartPointLatitude,
                o.StartPointLongitude, o.EndPointLatitude, o.EndPointLongitude, o.CargoVolume, o.CargoWeight));
        }
        else
        {
            getPrimaryOrderResponses = _ => null;
            getSecondaryOrderResponses = _ => null;
        }

        return (await driverStorageService.FindAll(filter, includedData)).Select(d => new Dtos.Driver.Response(d.Guid,
            d.HireDate, d.DismissalDate, d.HoursWorkedPerWeek, d.TotalHoursWorked,
            d.AdrQualificationFlag == null
                ? null
                : AdrDriverQualificationsFlags.FlagToString(d.AdrQualificationFlag.Value), d.AdrQualificationOfTank,
            d.Name, d.IsAvailable, getBranchResponse(d), getPrimaryOrderResponses(d), getSecondaryOrderResponses(d)));
    }
    
    public async Task DeleteDrivers(Expression<Func<Entities.Driver, bool>> filter)
    {
        var drivers = await driverStorageService.FindAll(filter);
        
        await driverStorageService.RemoveRange(drivers);
    }

    public async Task UpdateDrivers(IReadOnlyCollection<Dtos.Driver.UpdateRequest> updateRequests)
    {
        var driverGuids = new HashSet<string>(updateRequests.Count);
        foreach (var updateRequest in updateRequests)
            driverGuids.Add(updateRequest.Guid);
        var drivers = (await driverStorageService.FindAll(d => driverGuids.Contains(d.Guid))).ToDictionary(d => d.Guid);
        
        foreach (var updateRequest in updateRequests)
        {
            if (!drivers.TryGetValue(updateRequest.Guid, out var driver))
                throw new ArgumentException($"The driver {updateRequest.Guid} does not exist.", nameof(updateRequests));
            
            if (updateRequest.PropertyIsSet(nameof(updateRequest.Reinstate)))
                driver.Reinstate();
            if (updateRequest.PropertyIsSet(nameof(updateRequest.Dismiss)))
                driver.Dismiss();
            if (updateRequest.PropertyIsSet(nameof(updateRequest.AddHoursWorked)))
                driver.AddHoursWorked(updateRequest.AddHoursWorked);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.ResetHoursWorkedPerWeek)))
                driver.ResetHoursWorkedPerWeek();
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetAdrQualificationFlag)))
                driver.SetAdrQualificationFlag(updateRequest.SetAdrQualificationFlag == null
                    ? null
                    : AdrDriverQualificationsFlags.StringToFlag(updateRequest.SetAdrQualificationFlag));
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetAdrQualificationOfTank)))
                driver.SetAdrQualificationOfTank(updateRequest.SetAdrQualificationOfTank!.Value);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetBranch)))
            {
                var branch = await branchStorageService.Find(b => b.Guid == updateRequest.SetBranch);
                if (branch == null)
                    throw new ArgumentException($"The branch {updateRequest.SetBranch} does not exist.", nameof(updateRequests));
                
                driver.SetBranch(branch);
            }
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetName)))
                driver.SetName(updateRequest.SetName);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetIsAvailable)))
                driver.SetIsAvailable(updateRequest.SetIsAvailable!.Value);
        }
        
        await driverStorageService.UpdateRange(drivers.Values);
    }
    
    public async Task CreateTrucks(IReadOnlyCollection<Dtos.Truck.CreateRequest> createRequests)
    {
        var branchGuids = new HashSet<string>(createRequests.Count);
        foreach (var createRequest in createRequests)
            branchGuids.Add(createRequest.BranchGuid);
        var branches = (await branchStorageService.FindAll(b => branchGuids.Contains(b.Guid))).ToDictionary(b => b.Guid);

        var trucks = new List<Entities.Truck>(createRequests.Count);
        foreach (var createRequest in createRequests)
        {
            if (!branches.TryGetValue(createRequest.BranchGuid, out var branch))
                throw new ArgumentException($"The branch {createRequest.BranchGuid} does not exist.",
                    nameof(createRequests));

            trucks.Add(Entities.Truck.New(createRequest.Number, createRequest.TrailerIsTank, createRequest.VolumeMax,
                createRequest.VolumePrice, createRequest.WeightMax, createRequest.WeightPrice, createRequest.PricePerKm,
                branch, createRequest.PermittedHazardClassesFlags == null ? null : HazardClassesFlags.StringToFlagCombination(createRequest.PermittedHazardClassesFlags)));
        }
        
        await truckStorageService.CreateRange(trucks);
    }

    public async Task<IEnumerable<Dtos.Truck.Response>> GetTrucks(Expression<Func<Entities.Truck, bool>> filter, bool includeBranch, bool includeOrders)
    {
        var includedData = "";
        Func<Entities.Truck, Dtos.Branch.Response?> getBranchResponse;
        if (includeBranch)
        {
            includedData += "Branch;";
            getBranchResponse = t =>
                new Dtos.Branch.Response(t.Branch.Guid, t.Branch.Address, t.Branch.Latitude, t.Branch.Longitude, null,
                    null, null, null);
        }
        else
            getBranchResponse = _ => null;
        
        Func<Entities.Truck, IEnumerable<Dtos.Order.Response>?> getOrderResponses;
        if (includeOrders)
        {
            includedData += "Orders;";
            getOrderResponses = t => t.Orders.Select(o => new Dtos.Order.Response(o.Guid, o.Status, o.DateCreated,
                o.DateAssignmentOfPerformers, o.DatePaymentAndBegin, o.DateEnd, o.HazardClassFlag == null ? null : HazardClassesFlags.FlagCombinationToString(o.HazardClassFlag.Value), o.TankRequired, o.LengthInKm,
                o.Price, o.ExpectedHoursWorkedByDrivers, o.ActualHoursWorkedByDriver1, o.ActualHoursWorkedByDriver2,
                null, null, null, null, null, o.StartAddress, o.EndAddress, o.CargoDescription, o.StartPointLatitude,
                o.StartPointLongitude, o.EndPointLatitude, o.EndPointLongitude, o.CargoVolume, o.CargoWeight));
        }
        else
            getOrderResponses = _ => null;

        return (await truckStorageService.FindAll(filter, includedData)).Select(t => new Dtos.Truck.Response(t.Guid,
            t.CommissionedDate, t.DecommissionedDate, t.PermittedHazardClassesFlags == null ? null : HazardClassesFlags.FlagCombinationToString(t.PermittedHazardClassesFlags.Value), t.Number, t.IsAvailable,
            t.TrailerIsTank, t.VolumeMax, t.VolumePrice, t.WeightMax, t.WeightPrice, t.PricePerKm, getBranchResponse(t),
            getOrderResponses(t)));
    }
    
    public async Task DeleteTrucks(Expression<Func<Entities.Truck, bool>> filter)
    {
        var trucks = await truckStorageService.FindAll(filter);
        
        await truckStorageService.RemoveRange(trucks);
    }

    public async Task UpdateTrucks(IReadOnlyCollection<Dtos.Truck.UpdateRequest> updateRequests)
    {
        var trucksGuids = new HashSet<string>(updateRequests.Count);
        foreach (var updateRequest in updateRequests)
            trucksGuids.Add(updateRequest.Guid);
        var trucks = (await truckStorageService.FindAll(t => trucksGuids.Contains(t.Guid))).ToDictionary(d => d.Guid);
        
        foreach (var updateRequest in updateRequests)
        {
            if (!trucks.TryGetValue(updateRequest.Guid, out var truck))
                throw new ArgumentException($"The truck {updateRequest.Guid} does not exist.", nameof(updateRequests));
            
            if (updateRequest.PropertyIsSet(nameof(updateRequest.Recommission)))
                truck.Recommission();
            if (updateRequest.PropertyIsSet(nameof(updateRequest.Decommission)))
                truck.Decommission();
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetPermittedHazardClassesFlags)))
                truck.SetPermittedHazardClassesFlags(updateRequest.SetPermittedHazardClassesFlags == null ? null : HazardClassesFlags.StringToFlagCombination(updateRequest.SetPermittedHazardClassesFlags));
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetBranch)))
            {
                var branch = await branchStorageService.Find(b => b.Guid == updateRequest.SetBranch);
                if (branch == null)
                    throw new ArgumentException($"The branch {updateRequest.SetBranch} does not exist.", nameof(updateRequests));
                
                truck.SetBranch(branch);
            }
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetNumber)))
                truck.SetNumber(updateRequest.SetNumber);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetIsAvailable)))
                truck.SetIsAvailable(updateRequest.SetIsAvailable!.Value);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetTrailerIsTank)))
                truck.SetTrailerIsTank(updateRequest.SetTrailerIsTank!.Value);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetVolumeMax)))
                truck.SetVolumeMax(updateRequest.SetVolumeMax);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetVolumePrice)))
                truck.SetVolumePrice(updateRequest.SetVolumePrice);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetWeightMax)))
                truck.SetWeightMax(updateRequest.SetWeightMax);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetWeightPrice)))
                truck.SetWeightPrice(updateRequest.SetWeightPrice);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetPricePerKm)))
                truck.SetPricePerKm(updateRequest.SetPricePerKm);
        }
        
        await truckStorageService.UpdateRange(trucks.Values);
    }
    
    public async Task CreateVkUsers(IReadOnlyCollection<Dtos.User.CreateVkRequest> createRequests)
    {
        var users = createRequests.Select(cr => Entities.User.New(cr.Name, cr.Contact, cr.VkUserId));

        await userStorageService.CreateRange(users);
    }
    
    public async Task CreateStandartUsers(IReadOnlyCollection<Dtos.User.CreateStandartRequest> createRequests)
    {
        var users = createRequests.Select(cr => Entities.User.New(cr.Name, cr.Contact, cr.Login, cr.Password, cryptographicService));

        await userStorageService.CreateRange(users);
    }

    public async Task<IEnumerable<Dtos.User.Response>> GetUsers(Expression<Func<Entities.User, bool>> filter, bool includeOrders)
    {
        var includedData = "";
        Func<Entities.User, IEnumerable<Dtos.Order.Response>?> getOrderResponses;
        if (includeOrders)
        {
            includedData += "Orders;";
            getOrderResponses = u => u.Orders.Select(o => new Dtos.Order.Response(o.Guid, o.Status, o.DateCreated,
                o.DateAssignmentOfPerformers, o.DatePaymentAndBegin, o.DateEnd, o.HazardClassFlag == null ? null : HazardClassesFlags.FlagCombinationToString(o.HazardClassFlag.Value), o.TankRequired, o.LengthInKm,
                o.Price, o.ExpectedHoursWorkedByDrivers, o.ActualHoursWorkedByDriver1, o.ActualHoursWorkedByDriver2,
                null, null, null, null, null, o.StartAddress, o.EndAddress, o.CargoDescription, o.StartPointLatitude,
                o.StartPointLongitude, o.EndPointLatitude, o.EndPointLongitude, o.CargoVolume, o.CargoWeight));
        }
        else
            getOrderResponses = _ => null;

        return (await userStorageService.FindAll(filter, includedData)).Select(u => new Dtos.User.Response(u.Guid, u.RegistrationDate,
            u.VkUserId, u.Login, u.Password, u.Name, u.Contact, getOrderResponses(u)));
    }
    
    public async Task DeleteUsers(Expression<Func<Entities.User, bool>> filter)
    {
        var users = await userStorageService.FindAll(filter);
        
        await userStorageService.RemoveRange(users);
    }

    public async Task UpdateUsers(IReadOnlyCollection<Dtos.User.UpdateRequest> updateRequests)
    {
        var userGuids = new HashSet<string>(updateRequests.Count);
        foreach (var updateRequest in updateRequests)
            userGuids.Add(updateRequest.Guid);
        var users = (await userStorageService.FindAll(t => userGuids.Contains(t.Guid))).ToDictionary(d => d.Guid);
        
        foreach (var updateRequest in updateRequests)
        {
            if (!users.TryGetValue(updateRequest.Guid, out var user))
                throw new ArgumentException($"The user {updateRequest.Guid} does not exist.", nameof(updateRequests));
            
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetLogin)))
                user.SetLogin(updateRequest.SetLogin);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetPassword)))
                user.SetPassword(cryptographicService, updateRequest.SetPassword);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetName)))
                user.Name = updateRequest.SetName;
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetContact)))
                user.Contact = updateRequest.SetContact;
        }
        
        await userStorageService.UpdateRange(users.Values);
    }
    
    public async Task CreateBranches(IReadOnlyCollection<Dtos.Branch.CreateRequest> createRequests)
    {
        var branches = createRequests.Select(cr => Entities.Branch.New(cr.Address, (cr.Latitude, cr.Longitude)));

        await branchStorageService.CreateRange(branches);
    }

    public async Task<IEnumerable<Dtos.Branch.Response>> GetBranches(Expression<Func<Entities.Branch, bool>> filter, bool includeTrucks, bool includeDrivers)
    {
        var includedData = "";
        Func<Entities.Branch, IEnumerable<Dtos.Truck.Response>?> getTruckResponses;
        if (includeTrucks)
        {
            includedData += "Trucks;";
            getTruckResponses = b => b.Trucks.Select(t => new Dtos.Truck.Response(t.Guid, t.CommissionedDate,
                t.DecommissionedDate, t.PermittedHazardClassesFlags == null ? null : HazardClassesFlags.FlagCombinationToString(t.PermittedHazardClassesFlags.Value), t.Number, t.IsAvailable, t.TrailerIsTank,
                t.VolumeMax, t.VolumePrice, t.WeightMax, t.WeightPrice, t.PricePerKm, null, null));
        }
        else
            getTruckResponses = _ => null;
        
        Func<Entities.Branch, IEnumerable<Dtos.Driver.Response>?> getDriverResponses;
        if (includeDrivers)
        {
            includedData += "Drivers;";
            getDriverResponses = b => b.Drivers.Select(d => new Dtos.Driver.Response(d.Guid, d.HireDate,
                d.DismissalDate, d.HoursWorkedPerWeek, d.TotalHoursWorked,
                d.AdrQualificationFlag == null
                    ? null
                    : AdrDriverQualificationsFlags.FlagToString(d.AdrQualificationFlag.Value),
                d.AdrQualificationOfTank, d.Name, d.IsAvailable, null, null, null));
        }
        else
            getDriverResponses = _ => null;

        return (await branchStorageService.FindAll(filter, includedData)).Select(b =>
            new Dtos.Branch.Response(b.Guid, b.Address, b.Latitude, b.Longitude, getTruckResponses(b),
                getDriverResponses(b), null, null));
    }
    
    public async Task DeleteBranches(Expression<Func<Entities.Branch, bool>> filter)
    {
        var branches = await branchStorageService.FindAll(filter);
        
        await branchStorageService.RemoveRange(branches);
    }

    public async Task UpdateBranches(IReadOnlyCollection<Dtos.Branch.UpdateRequest> updateRequests)
    {
        var branchGuids = new HashSet<string>(updateRequests.Count);
        foreach (var updateRequest in updateRequests)
            branchGuids.Add(updateRequest.Guid);
        var branches = (await branchStorageService.FindAll(b => branchGuids.Contains(b.Guid))).ToDictionary(d => d.Guid);
        
        foreach (var updateRequest in updateRequests)
        {
            if (!branches.TryGetValue(updateRequest.Guid, out var branch))
                throw new ArgumentException($"The branch {updateRequest.Guid} does not exist.", nameof(updateRequests));
            
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetAddress)))
                branch.Address = updateRequest.SetAddress;
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetLatitude)))
                branch.Latitude = updateRequest.SetLatitude!.Value;
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetLongitude)))
                branch.Longitude = updateRequest.SetLongitude!.Value;
        }
        
        await branchStorageService.UpdateRange(branches.Values);
    }
    
    /*public IEnumerable<Dtos.Branch.Response> GetOrders(Expression<Func<Entities.Branch, bool>> filter, bool includeTrucks, bool includeDrivers)
    {
        var includedData = "";
        Func<Entities.Branch, IEnumerable<Dtos.Truck.Response>?> getTruckResponses;
        if (includeTrucks)
        {
            includedData += "Trucks;";
            getTruckResponses = b => b.Trucks.Select(t => new Dtos.Truck.Response(t.Guid, t.CommissionedDate,
                t.DecommissionedDate, t.PermittedHazardClassesFlags == null ? null : HazardClassesFlags.FlagCombinationToString(t.PermittedHazardClassesFlags.Value), t.Number, t.IsAvailable, t.TrailerIsTank,
                t.VolumeMax, t.VolumePrice, t.WeightMax, t.WeightPrice, t.PricePerKm, null, null));
        }
        else
            getTruckResponses = _ => null;
        
        Func<Entities.Branch, IEnumerable<Dtos.Driver.Response>?> getDriverResponses;
        if (includeDrivers)
        {
            includedData += "Drivers;";
            getDriverResponses = b => b.Drivers.Select(d => new Dtos.Driver.Response(d.Guid, d.HireDate,
                d.DismissalDate, d.HoursWorkedPerWeek, d.TotalHoursWorked,
                d.AdrQualificationFlag == null
                    ? null
                    : AdrDriverQualificationsFlags.FlagToString(d.AdrQualificationFlag.Value),
                d.AdrQualificationOfTank, d.Name, d.IsAvailable, null, null));
        }
        else
            getDriverResponses = _ => null;

        return branchStorageService.FindAll(filter, includedData).Select(b =>
            new Dtos.Branch.Response(b.Guid, b.Address, b.Latitude, b.Longitude, getTruckResponses(b),
                getDriverResponses(b), null, null));
    }*/
    
    public async Task<IEnumerable<Dtos.Branch.Response>> GetPotentialOrderPerformersByBranches(string orderGuid)
    {
        var order = await orderStorageService.Find(o => o.Guid == orderGuid);
        if (order == null)
            throw new ArgumentException($"The orded {orderGuid} does not exist.", nameof(orderGuid));
        
        Func<Entities.Truck, bool> truckPredicate;
        Func<Entities.Driver, bool> driverPredicate;
        if (order.HazardClassFlag != null)
        {
            truckPredicate = t =>
                t.IsAvailable && t.TrailerIsTank == order.TankRequired &&
                (order.HazardClassFlag & t.PermittedHazardClassesFlags ?? 0) > 0;
            if (order.TankRequired)
                driverPredicate = d =>
                    d.IsAvailable && (order.HazardClassFlag & d.AdrQualificationFlag ?? 0) > 1 &&
                    d.AdrQualificationOfTank;
            else
                driverPredicate = d => d.IsAvailable && (order.HazardClassFlag & d.AdrQualificationFlag ?? 0) > 1;
        }
        else
        {
            truckPredicate = t =>
                t.IsAvailable && t.TrailerIsTank == order.TankRequired;
            driverPredicate = d => d.IsAvailable;
        }

        var branches = await branchStorageService.FindAll(_ => true, "Trucks;Drivers;");
        var branchResponses = new List<Dtos.Branch.Response>(branches.Count);
        foreach (var branch in branches)
        {
            var trucks = branch.Trucks.Where(t => truckPredicate(t)).Select(t => new Dtos.Truck.Response(t.Guid, t.CommissionedDate,
                t.DecommissionedDate, t.PermittedHazardClassesFlags == null ? null : HazardClassesFlags.FlagCombinationToString(t.PermittedHazardClassesFlags.Value), t.Number, t.IsAvailable, t.TrailerIsTank,
                t.VolumeMax, t.VolumePrice, t.WeightMax, t.WeightPrice, t.PricePerKm, null, null));
            
            var drivers = branch.Drivers.Where(d => driverPredicate(d)).Select(d => new Dtos.Driver.Response(d.Guid, d.HireDate,
                d.DismissalDate, d.HoursWorkedPerWeek, d.TotalHoursWorked,
                d.AdrQualificationFlag == null
                    ? null
                    : AdrDriverQualificationsFlags.FlagToString(d.AdrQualificationFlag.Value),
                d.AdrQualificationOfTank, d.Name, d.IsAvailable, null, null, null));
            
            var lengthInKmAndDrivingHours = branch
                .CalculateLengthInKmOfOrderRouteClosedAtBranchAndApproximateDrivingHoursOfTruckAlongIt(order,
                    geolocationService);
           
            branchResponses.Add(new Dtos.Branch.Response(branch.Guid, branch.Address, branch.Latitude, branch.Longitude,
                trucks, drivers, lengthInKmAndDrivingHours.LengthInKm,lengthInKmAndDrivingHours.DrivingHours));
        }

        return branchResponses;
    }
}