using Application.Interfaces;

namespace Application.Commands.Driver;

public class QualifyAdr : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver entity) => entity.QualifyAdr(AdrQualificationFlag);
    
    public required int AdrQualificationFlag { get; init; }
}