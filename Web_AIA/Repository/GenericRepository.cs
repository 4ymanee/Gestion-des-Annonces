using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Web_AIA.Data;

namespace Web_AIA.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly MyDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(MyDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await DbSet.Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

    public void Update(T entity) => DbSet.Update(entity);

    public void Remove(T entity) => DbSet.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Context.SaveChangesAsync(cancellationToken);
}
