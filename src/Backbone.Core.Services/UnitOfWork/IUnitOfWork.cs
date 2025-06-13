using System;
using System.Threading.Tasks;

namespace Backbone.Core.Services.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int Commit();
        public Task<int> CommitAsync();
    }
}