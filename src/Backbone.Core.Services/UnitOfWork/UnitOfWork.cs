using Microsoft.EntityFrameworkCore;

namespace Backbone.Core.Services.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext _dbContext;
        public UnitOfWork(DbContext context)
        {
            _dbContext = context;
            System.Diagnostics.Debug.WriteLine("DbContent ID:" + (context as object).GetHashCode());
        }

        public int Commit()
        {
            return _dbContext.SaveChanges();
        }
        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                    _dbContext = null;
                }
            }
        }
    }
}