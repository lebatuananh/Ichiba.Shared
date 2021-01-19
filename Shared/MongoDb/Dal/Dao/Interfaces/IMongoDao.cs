using System.Collections.Generic;
using System.Threading.Tasks;
using Hinox.Data.Mongo.Dal.Entities;
using MongoDB.Driver;
using Shared.MongoDb.Filters;

namespace Hinox.Data.Mongo.Dal.Dao.Interfaces
{
    public interface IMongoDao<T, TId> where T : IMongoEntity<TId>
    {
        IMongoCollection<T> Collection { get; }
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(TId id);
        Task AddAsync(T dto);
        Task AddAsync(IEnumerable<T> listDto);
        Task<T> UpdateAsync(T dto);
        Task<T> DeleteAsync(T dto);
        Task<List<T>> FilterAsync(BaseMdPagingFilter<T> mdFilter);
        Task<int> CountAsync(BaseMdPagingFilter<T> mdFilter);
    }
}