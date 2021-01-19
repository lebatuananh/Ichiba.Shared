using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shared.MongoDb.Filters;

namespace Shared.MongoDb
{
    public interface IMongoRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity obj);

        Task AddRangeAsync(IEnumerable<TEntity> objs);

        Task<TEntity> GetByIdAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> filter);

        Task<TEntity> UpdateAsync(int id, TEntity obj);

        Task<bool> RemoveAsync(int id);

        Task<List<TEntity>> FilterAsync(BaseMdPagingFilter<TEntity> mdFilter);

        Task<int> CountAsync(BaseMdPagingFilter<TEntity> mdFilter);

        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter);

        IMongoCollection<TEntity> Collection { get; }
    }
}