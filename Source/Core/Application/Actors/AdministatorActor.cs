using Application.Dtos;
using Domain.Entities;
using Domain.ServiceInterfaces;

namespace Application.Actors;

public class AdministatorActor(IEntityStorageService<Driver> driverStorageService, IEntityStorageService<Branch> branchStorageService)
{
    public void RegisterDriver(DriverRegistrationRequestDto requestDto)
    {
        var branch = branchStorageService.Find(b => b.Address == requestDto.BranchAddress);
        if (branch == null)
            throw new ArgumentOutOfRangeException(nameof(requestDto), requestDto,
                "The branch repository does not contain the specified branch.");

        driverStorageService.Create(Driver.New(requestDto.Name, requestDto.CertificatAdr, branch));
    }
}