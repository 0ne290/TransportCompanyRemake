using System.Linq.Expressions;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Interactors;

public class Order(IEntityStorageService<Domain.Entities.Driver> driverStorageService, IEntityStorageService<Domain.Entities.Branch> branchStorageService)
{
    public void Create(IReadOnlyCollection<DriverCreateRequest> createRequests)
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
    }

    public IEnumerable<DriverResponse> Get(Expression<Func<Domain.Entities.Driver, bool>> filter, bool includeBranch, bool includeOrders)
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

        return driverStorageService.AsNoTracking().FindAll(filter).Select(d => new DriverResponse(d.Guid, d.HireDate,
            d.DismissalDate, d.HoursWorkedPerWeek, d.TotalHoursWorked, d.AdrQualificationFlag, d.AdrQualificationOfTank,
            d.Name, d.IsAvailable, getBranchResponse(d), getOrderResponses(d)));
    }
    
    public void Delete(Expression<Func<Domain.Entities.Driver, bool>> filter) => driverStorageService.RemoveAll(filter);

    public void Reinstate() => _currentDriver.Reinstate();

    public void Dismiss() => _currentDriver.Dismiss();
    
    public void AddHoursWorked(double hoursWorked) => _currentDriver.AddHoursWorked(hoursWorked);

    public void ResetHoursWorkedPerWeek() => _currentDriver.ResetHoursWorkedPerWeek();
    
    public void SetAdrQualificationFlag(int? adrQualificationFlag) => _currentDriver.SetAdrQualificationFlag(adrQualificationFlag);

    public void SetAdrQualificationOfTank(bool adrQualificationOfTank) => _currentDriver.SetAdrQualificationOfTank(adrQualificationOfTank);
    
    public void SetBranch(string branchGuid)
    {
        var branch = branchStorageService.Find(b => b.Guid == branchGuid);
        
        if (branch == null)
            throw new ArgumentException($"The branch {branchGuid} does not exist.", nameof(branchGuid));
        
        _currentDriver.SetBranch(branch);
    }

    public void SetName(string name) => _currentDriver.SetName(name);
    
    public void SetIsAvailable(bool isAvailable) => _currentDriver.SetIsAvailable(isAvailable);

    public void SaveChanges() => driverStorageService.SaveChanges();

    public string CurrentDriverGuid
    {
        get => _currentDriverGuid;
        set
        {
            var driver = driverStorageService.Find(d => d.Guid == value);

            _currentDriver = driver ?? throw new ArgumentException($"Truck {value} does not exist", nameof(value));
            _currentDriverGuid = value;
        }
    }

    private string _currentDriverGuid = null!;

    private Domain.Entities.Driver _currentDriver = null!;
}