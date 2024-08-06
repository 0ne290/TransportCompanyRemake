using System.Linq.Expressions;
using Application.Dtos;
using Application.Dtos.Driver;
using Application.Interfaces;

namespace Application.Actors;

public class Administrator(IEntityStorageService<Domain.Entities.Driver> driverStorageService, IEntityStorageService<Domain.Entities.Branch> branchStorageService)
{
    public void CreateDrivers(IReadOnlyCollection<CreateRequest> createRequests)
    {
        var branchGuids = new HashSet<string>(createRequests.Count);
        foreach (var createRequest in createRequests)
            branchGuids.Add(createRequest.BranchGuid);
        var branches = branchStorageService.AsNoTracking().FindAll(b => branchGuids.Contains(b.Guid)).ToDictionary(b => b.Guid);

        var drivers = new List<Domain.Entities.Driver>(createRequests.Count);
        foreach (var createRequest in createRequests)
        {
            if (!branches.TryGetValue(createRequest.BranchGuid, out var branch))
                throw new ArgumentException($"The branch {createRequest.BranchGuid} does not exist.",
                    nameof(createRequests));
            
            drivers.Add(Domain.Entities.Driver.New(createRequest.Name, branch, createRequest.AdrQualificationFlag, createRequest.AdrQualificationOfTank));
        }
        
        driverStorageService.CreateRange(drivers);
        driverStorageService.SaveChanges();
    }

    public IEnumerable<Response> GetDrivers(Expression<Func<Domain.Entities.Driver, bool>> filter, bool includeBranch, bool includeOrders)
    {
        Func<Domain.Entities.Driver, BranchResponse?> getBranchResponse;
        if (includeBranch)
            getBranchResponse = d =>
                new BranchResponse(d.Branch.Guid, d.Branch.Address, d.Branch.Latitude, d.Branch.Longitude, null, null);
        else
            getBranchResponse = _ => null;
        
        Func<Domain.Entities.Driver, IEnumerable<OrderResponse>?> getOrderResponses;
        if (includeOrders)
            getOrderResponses = d => d.Orders.Select(o => new OrderResponse(o.Guid, o.Status, o.DateCreated,
                o.DateAssignmentOfPerformers, o.DatePaymentAndBegin, o.DateEnd, o.HazardClassFlag, o.Tank, o.LengthInKm,
                o.Price, o.ExpectedHoursWorkedByDrivers, o.ActualHoursWorkedByDriver1, o.ActualHoursWorkedByDriver2,
                null, null, null, null, null, o.StartAddress, o.EndAddress, o.CargoDescription, o.StartPointLatitude,
                o.StartPointLongitude, o.EndPointLatitude, o.EndPointLongitude, o.CargoVolume, o.CargoWeight));
        else
            getOrderResponses = _ => null;

        return driverStorageService.AsNoTracking().FindAll(filter).Select(d => new Response(d.Guid, d.HireDate,
            d.DismissalDate, d.HoursWorkedPerWeek, d.TotalHoursWorked, d.AdrQualificationFlag, d.AdrQualificationOfTank,
            d.Name, d.IsAvailable, getBranchResponse(d), getOrderResponses(d)));
    }
    
    public void DeleteDrivers(Expression<Func<Domain.Entities.Driver, bool>> filter)
    {
        var drivers = driverStorageService.AsNoTracking().FindAll(filter);
        
        driverStorageService.RemoveAll(drivers);
        driverStorageService.SaveChanges();
    }

    public void UpdateDrivers(IReadOnlyCollection<UpdateRequest> updateRequests)
    {
        var driverGuids = new HashSet<string>(updateRequests.Count);
        foreach (var updateRequest in updateRequests)
            driverGuids.Add(updateRequest.Guid);
        var drivers = driverStorageService.FindAll(d => driverGuids.Contains(d.Guid)).ToDictionary(d => d.Guid);
        
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
                driver.SetAdrQualificationFlag(updateRequest.SetAdrQualificationFlag);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetBranch)))
            {
                var branch = branchStorageService.AsNoTracking().Find(b => b.Guid == updateRequest.SetBranch);
                if (branch == null)
                    throw new ArgumentException($"The branch {updateRequest.SetBranch} does not exist.", nameof(updateRequests));
                
                driver.SetBranch(branch);
            }
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetName)))
                driver.SetName(updateRequest.SetName);
            if (updateRequest.PropertyIsSet(nameof(updateRequest.SetIsAvailable)))
                driver.SetIsAvailable(updateRequest.SetIsAvailable!.Value);
        }
        
        driverStorageService.SaveChanges();
    }
}