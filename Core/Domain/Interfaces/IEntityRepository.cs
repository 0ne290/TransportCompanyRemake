using Domain.Entities;

namespace Domain.Interfaces;

public interface IEntityRepository<TEntity> : IDisposable, IAsyncDisposable
{
    TEntity? Find(Predicate<TEntity> predicate);
    
    bool Exists(Predicate<TEntity> predicate);
    
    bool Contains(TEntity entity);

    void Create(TEntity entity);
}