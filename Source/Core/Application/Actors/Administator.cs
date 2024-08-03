using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Actors;
public class Administator(IEntityStorageService<Driver> driverStorageService, IEntityStorageService<Truck> truckStorageService, IEntityStorageService<Branch> branchStorageService)
{
    public void CreateDrivers(IEnumerable<DriverCreateRequest> createRequests)
    {
        if (!driverStorageService.Create(createRequests.Select(cr =>
                Driver.New(cr.Name, cr.BranchGuid, cr.AdrQualificationFlag, cr.AdrQualificationOfTank))))
            throw new ArgumentException(
                "Unable to create drivers. Probably one of the creation requests refers to a non-existent branch.",
                nameof(createRequests));
    }

    public void UpdateDrivers(IEnumerable<DriverUpdateRequest> updateRequests)
    {
        foreach (var updateRequest in updateRequests)
        {
            var driver = driverStorageService.FindByPrimaryKey(new object[] { updateRequest.DriverGuid });

            if (driver == null)
                throw new ArgumentException($"The driver {updateRequest.DriverGuid} does not exist.", nameof(updateRequests));

            foreach (var updateAction in updateRequest.Updater)
                updateAction(driver);
        }
        
        if (!driverStorageService.SaveChanges())
            throw new ArgumentException("Unable to save changes made by drivers updaters. Probably one of the \"SetBranch\" update actions is referencing a non-existent branch.", nameof(updateRequests));
    }

    public void DeleteDrivers(IEnumerable<string> guids) => driverStorageService.RemoveAll(d => guids.Contains(d.Guid));
    
    public void CreateTrucks(IEnumerable<TruckCreateRequest> createRequests)
    {
        if (!driverStorageService.Create(createRequests.Select(cr =>
                Driver.New(cr.Name, cr.BranchGuid, cr.AdrQualificationFlag, cr.AdrQualificationOfTank))))
            throw new ArgumentException(
                "Unable to create drivers. Probably one of the creation requests refers to a non-existent branch.",
                nameof(createRequests));
    }

    public void UpdateTrucks(IEnumerable<TrucksUpdateRequest> updateRequests)
    {
        foreach (var updateRequest in updateRequests)
        {
            var driver = driverStorageService.FindByPrimaryKey(new object[] { updateRequest.DriverGuid });

            if (driver == null)
                throw new ArgumentException($"The driver {updateRequest.DriverGuid} does not exist.", nameof(updateRequests));

            foreach (var updateAction in updateRequest.Updater)
                updateAction(driver);
        }
        
        if (!driverStorageService.SaveChanges())
            throw new ArgumentException("Unable to save changes made by drivers updaters. Probably one of the \"SetBranch\" update actions is referencing a non-existent branch.", nameof(updateRequests));
    }

    public void DeleteTrucks(IEnumerable<string> guids) => driverStorageService.RemoveAll(d => guids.Contains(d.Guid));

    public IEnumerable<BranchResponse> GetAllBranches()
    {
        var branches = branchStorageService.AsNoTracking().AsEnumerable().ToList();

        return branches.Select(b => new BranchResponse(b.Guid, b.Address, b.Latitude, b.Longitude,
            b.Trucks.Select(t => new TruckResponse(t.Guid, t.CommissionedDate, t.DecommissionedDate,
                t.PermittedHazardClassesFlags, t.Number, t.IsAvailable, t.TrailerIsTank, t.VolumeMax, t.VolumePrice,
                t.WeightMax, t.WeightPrice, t.PricePerKm, null, null)),
            b.Drivers.Select(d => new DriverResponse(d.Guid, d.HireDate, d.DismissalDate, d.HoursWorkedPerWeek,
                d.TotalHoursWorked, d.AdrQualificationFlag, d.AdrQualificationOfTank, d.Name, d.IsAvailable, null,
                null))));
    }
}