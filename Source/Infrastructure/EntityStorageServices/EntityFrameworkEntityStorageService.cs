using System.Linq.Expressions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityStorageServices;

public class EntityFrameworkEntityStorageService<TEntity>(TransportCompanyContext dbContext) : IEntityStorageService<TEntity> where TEntity : class
{
    public async Task CreateRange(IEnumerable<TEntity> entities)
    {
        await dbContext.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task<TEntity?> Find(Expression<Func<TEntity, bool>> filter, string includedData = "")
    {
        var includedProperties = includedData.Split(";");
        
        var query = dbContext.Set<TEntity>().AsNoTrackingWithIdentityResolution();
        query = includedProperties.Where(includedProperty => !string.IsNullOrEmpty(includedProperty))
            .Aggregate(query, (current, includedProperty) => current.Include(includedProperty));

        return await query.SingleOrDefaultAsync(filter);
    }

    public async Task<ICollection<TEntity>> FindAll(Expression<Func<TEntity, bool>> filter, string includedData = "")
    {
        var includedProperties = includedData.Split(";");
        
        var query = dbContext.Set<TEntity>().AsNoTrackingWithIdentityResolution();
        query = includedProperties.Where(includedProperty => !string.IsNullOrEmpty(includedProperty))
            .Aggregate(query, (current, includedProperty) => current.Include(includedProperty));

        return await query.Where(filter).ToListAsync();
    }

    public async Task UpdateRange(IEnumerable<TEntity> entities)
    {
        var entityStore = dbContext.Set<TEntity>();
        entityStore.AttachRange();
        foreach (var entity in entities)
            dbContext.Entry(entity).State = EntityState.Modified;

        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveRange(IEnumerable<TEntity> entities)
    {
        dbContext.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}