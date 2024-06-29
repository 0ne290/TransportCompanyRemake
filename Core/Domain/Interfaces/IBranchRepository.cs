using Domain.Entities;

namespace Domain.Interfaces;

public interface IBranchRepository
{
    Branch? Find(Predicate<Branch> predicate);
    
    bool Exists(Predicate<Branch> predicate);
}