namespace Application.Interfaces;

public interface IEntityStorageService<TEntity> : IDisposable, IAsyncDisposable
{
    TEntity? FindByPrimaryKey(object[] primaryKey);
    
    void Create(TEntity entity);
    
    IEnumerable<TEntity> FindAll(Predicate<TEntity> predicate);
    
    bool Exists(Predicate<TEntity> predicate);
    
    bool Contains(TEntity entity);

    void RemoveAll(Predicate<TEntity> predicate);
    
    bool SaveChanges();
}