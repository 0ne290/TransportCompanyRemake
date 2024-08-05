using System.Linq.Expressions;
using Application.Dtos;
using Application.Interfaces;

namespace Application.Interactors;

public class Branch(IEntityStorageService<Domain.Entities.Branch> branchStorageService)
{
    public void Create(IReadOnlyCollection<BranchCreateRequest> createRequests) => branchStorageService.CreateRange(
        createRequests.Select(createRequest =>
            Domain.Entities.Branch.New(createRequest.Address, (createRequest.Latitude, createRequest.Longitude))));

    public IEnumerable<BranchResponse> Get(Expression<Func<Domain.Entities.Branch, bool>> filter, bool includeTrucks, bool includeDrivers)
    {
        Func<Domain.Entities.Branch, IEnumerable<TruckResponse>?> getTruckResponses;
        if (includeTrucks)
            getTruckResponses = b => b.Trucks.Select(t => new TruckResponse(t.Guid, t.CommissionedDate,
                t.DecommissionedDate, t.PermittedHazardClassesFlags, t.Number, t.IsAvailable, t.TrailerIsTank,
                t.VolumeMax, t.VolumePrice, t.WeightMax, t.WeightPrice, t.PricePerKm, null, null));
        else
            getTruckResponses = _ => null;
        
        Func<Domain.Entities.Branch, IEnumerable<DriverResponse>?> getDriverResponses;
        if (includeDrivers)
            getDriverResponses = b => b.Drivers.Select(d => new DriverResponse(d.Guid, d.HireDate, d.DismissalDate,
                d.HoursWorkedPerWeek, d.TotalHoursWorked, d.AdrQualificationFlag, d.AdrQualificationOfTank, d.Name,
                d.IsAvailable, null, null));
        else
            getDriverResponses = _ => null;

        return branchStorageService.AsNoTracking().FindAll(filter).Select(b =>
            new BranchResponse(b.Guid, b.Address, b.Latitude, b.Longitude, getTruckResponses(b),
                getDriverResponses(b)));
    }
    
    public void Delete(Expression<Func<Domain.Entities.Branch, bool>> filter) => branchStorageService.RemoveAll(filter);

    public void SetAddress(string address) => _currentBranch.Address = address;

    public void SetLatitude(double latitude) => _currentBranch.Latitude = latitude;
    
    public void SetLongitude(double longitude) => _currentBranch.Longitude = longitude;

    public void SaveChanges() => branchStorageService.SaveChanges();

    public string CurrentBranchGuid
    {
        get => _currentBranchGuid;
        set
        {
            var branch = branchStorageService.Find(d => d.Guid == value);

            _currentBranch = branch ?? throw new ArgumentException($"Branch {value} does not exist", nameof(value));
            _currentBranchGuid = value;
        }
    }

    private string _currentBranchGuid = null!;

    private Domain.Entities.Branch _currentBranch = null!;
}