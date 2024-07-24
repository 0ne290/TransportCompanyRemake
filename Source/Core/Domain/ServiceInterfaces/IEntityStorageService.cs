namespace Domain.ServiceInterfaces;

public interface IEntityStorageService<TEntity> : IDisposable, IAsyncDisposable
{
    TEntity? Find(Predicate<TEntity> predicate);
    
    bool Exists(Predicate<TEntity> predicate);
    
    bool Contains(TEntity entity);

    void Create(TEntity entity);
}