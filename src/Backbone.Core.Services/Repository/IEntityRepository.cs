using System.Linq.Expressions;

namespace Backbone.Core.Services.Repository
{
    public interface IEntityRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(bool noTracking = false);
        IQueryable<T> Query(bool noTracking = false);
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Edit(T entity);
        void EditRange(IEnumerable<T> entities);
        void Delete(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task SaveAsync();
    }

}