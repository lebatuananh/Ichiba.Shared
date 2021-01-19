using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.EF.Interfaces;

namespace Shared.Infrastructure
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private TContext _dbContext;

        public UnitOfWork(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public void Rollback()
        {
            Dispose();
        }

        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (_dbContext == null) return;

            _dbContext.Dispose();
            _dbContext = null;
        }
    }
}