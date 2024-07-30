using Application.Interfaces;

namespace Application.Commands.Driver;

public class SetBranch : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver entity) => entity.SetBranch(BranchGuid);
    
    public required string BranchGuid { get; init; }
}