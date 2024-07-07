using Application.Dtos;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Actors;

public class AdministatorActor(IEntityRepository<Driver> driverRepository, IEntityRepository<Branch> branchRepository)
{
    public void RegisterDriver(DriverRegistrationRequestDto requestDto)
    {
        var branch = branchRepository.Find(b => b.Address == requestDto.BranchAddress);
        if (branch == null)
            throw new ArgumentOutOfRangeException(nameof(requestDto), requestDto,
                "The branch repository does not contain the specified branch.");

        driverRepository.Create(Driver.New(requestDto.Name, requestDto.CertificatAdr, branch));
    }
}