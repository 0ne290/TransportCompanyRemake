namespace Application.Interfaces;

public interface IEntityStorageService<TEntity> : IDisposable, IAsyncDisposable
{
    IEntityStorageService<TEntity> AsNoTracking();
    
    void Create(IEnumerable<TEntity> entities);

    IEnumerable<TEntity> AsEnumerable();
    
    TEntity? FindByPrimaryKey(object[] primaryKey);
    
    IEnumerable<TEntity> FindAll(Predicate<TEntity> predicate);
    
    bool Exists(Predicate<TEntity> predicate);
    
    bool Contains(TEntity entity);

    void RemoveAll(Predicate<TEntity> predicate);
    
    void SaveChanges();
}