using Application.Commands;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Actors;

public class Administator(IEntityStorageService<Driver> driverStorageService, IEntityStorageService<Branch> branchStorageService)
{
    public void UpdateDrivers(IEnumerable<MacroCommand<Driver>> macroCommands)
    {
        foreach (var macroCommand in macroCommands)
        {
            var driver = driverStorageService.FindByPrimaryKey(new object[] { macroCommand.EntityGuid });

            if (driver == null)
                throw new ArgumentException("Attempt to execute macro command to non-existent driver.", nameof(macroCommands));
            
            macroCommand.Execute(driver);
        }
        
        if (!driverStorageService.SaveChanges())
            throw new ArgumentException("Unable to save changes made by macro commands. Probably one of the macro commands set the driver branch to a non-existent one.", nameof(macroCommands));
    }

    public void DeleteDrivers(IEnumerable<string> guids) => driverStorageService.RemoveAll(d => guids.Contains(d.Guid));
    
    /*public void RegisterDriver(DriverRegistrationRequestDto requestDto)
    {
        var branch = branchStorageService.Find(b => b.Address == requestDto.BranchAddress);
        if (branch == null)
            throw new ArgumentOutOfRangeException(nameof(requestDto), requestDto,
                "The branch repository does not contain the specified branch.");

        driverStorageService.Create(Driver.New(requestDto.Name, requestDto.CertificatAdr, branch));
    }*/
}