using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.EF.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
        int Commit();
        void Rollback();
    }

    public interface IUnitOfWork<TDbContext> : IUnitOfWork
    {
    }
}