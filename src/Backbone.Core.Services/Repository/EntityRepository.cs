using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backbone.Core.Services.Repository
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public EntityRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual async Task<List<T>> GetAllAsync(bool noTracking = false)
        {
            return noTracking
                ? await _dbSet.AsNoTracking().ToListAsync()
                : await _dbSet.ToListAsync();
        }

        public virtual IQueryable<T> Query(bool noTracking = false)
        {
            return noTracking ? _dbSet.AsNoTracking() : _dbSet;
        }

        public virtual async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Edit(T entity)
        {
            _dbSet.Update(entity);
        }
        public virtual void EditRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}