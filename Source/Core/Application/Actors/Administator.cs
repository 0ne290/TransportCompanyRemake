using Application.Commands;
using Application.Commands.Driver;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Actors;

public class Administator(IEntityStorageService<Driver> driverStorageService, IEntityStorageService<Branch> branchStorageService)
{
    public void CreateDrivers(IEnumerable<DriverRequest> creationRequests)
    {
        if (!driverStorageService.Create(creationRequests.Select(Driver.New)))
            throw new ArgumentException(
                "Unable to create drivers. Probably one of the creation requests refers to a non-existent branch.",
                nameof(creationRequests));
    }
    
    public void UpdateDrivers(IEnumerable<DriverMacroCommand> macroCommands)
    {
        foreach (var macroCommand in macroCommands)
        {
            var driver = driverStorageService.FindByPrimaryKey(new object[] { macroCommand.EntityGuid });

            if (driver == null)
                throw new ArgumentException("Unable to execute a driver macro command that references a driver that does not exist.", nameof(macroCommands));
            
            macroCommand.Execute(driver);
        }
        
        if (!driverStorageService.SaveChanges())
            throw new ArgumentException("Unable to save changes made by driver macro commands. Probably one of the \"SetBranch\" macro commands is referencing a non-existent branch.", nameof(macroCommands));
    }

    public void DeleteDrivers(IEnumerable<string> guids) => driverStorageService.RemoveAll(d => guids.Contains(d.Guid));

    public IEnumerable<BranchResponse> GetAllBranches()
    {
        var branches = branchStorageService.AsNoTracking().AsEnumerable();

        return branches.Select(b => new BranchResponse());
    }
    
    /*public void RegisterDriver(DriverRegistrationRequestDto requestDto)
    {
        var branch = branchStorageService.Find(b => b.Address == requestDto.BranchAddress);
        if (branch == null)
            throw new ArgumentOutOfRangeException(nameof(requestDto), requestDto,
                "The branch repository does not contain the specified branch.");

        driverStorageService.Create(Driver.New(requestDto.Name, requestDto.CertificatAdr, branch));
    }*/
}