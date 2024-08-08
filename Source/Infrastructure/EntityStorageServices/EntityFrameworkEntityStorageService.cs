using System.Linq.Expressions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityStorageServices;

public class EntityFrameworkEntityStorageService<TEntity>(TransportCompanyContext dbContext) : IEntityStorageService<TEntity> where TEntity : class
{
    public void CreateRange(IEnumerable<TEntity> entities)
    {
        dbContext.AddRangeAsync(entities);
        dbContext.SaveChangesAsync();
    }

    public TEntity? Find(Expression<Func<TEntity, bool>> filter, string includedData = "")
    {
        var includedProperties = includedData.Split(";");
        
        var query = dbContext.Set<TEntity>().AsNoTrackingWithIdentityResolution();
        query = includedProperties.Where(includedProperty => !string.IsNullOrEmpty(includedProperty))
            .Aggregate(query, (current, includedProperty) => current.Include(includedProperty));

        return query.SingleOrDefault(filter);
    }

    public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter, string includedData = "")
    {
        var includedProperties = includedData.Split(";");
        
        var query = dbContext.Set<TEntity>().AsNoTrackingWithIdentityResolution();
        query = includedProperties.Where(includedProperty => !string.IsNullOrEmpty(includedProperty))
            .Aggregate(query, (current, includedProperty) => current.Include(includedProperty));

        return query.Where(filter);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        var entityStore = dbContext.Set<TEntity>();
        entityStore.AttachRange();
        foreach (var entity in entities)
            dbContext.Entry(entity).State = EntityState.Modified;

        dbContext.SaveChangesAsync();
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        dbContext.RemoveRange(entities);
        dbContext.SaveChangesAsync();
    }
}