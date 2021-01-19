using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hinox.Data.Mongo.Dal.Dao.Interfaces;
using Hinox.Data.Mongo.Dal.Entities;
using MongoDB.Driver;
using Shared.MongoDb.Filters;

namespace Hinox.Data.Mongo.Dal.Dao
{
    public abstract class BaseMongoDao<T, TId> : IMongoDao<T, TId> where T : IMongoEntity<TId>
    {
        protected IMongoDatabase Database;

        public BaseMongoDao(IMongoDbFactory databaseFactory)
        {
            Database = databaseFactory.GetMongoDatabase<T>();
            var type = typeof(T);
            Collection = Database.GetCollection<T>(databaseFactory.GetCollectionName<T>());
        }

        public IMongoCollection<T> Collection { get; }

        public async Task<List<T>> GetAllAsync()
        {
            var filter = Builders<T>.Filter.Empty;
            var filterResult = await Collection.FindAsync(filter);
            var entities = await filterResult.ToListAsync();
            return entities;
        }

        public async Task<T> GetByIdAsync(TId id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var filterResult = await (await Collection.FindAsync(filter)).ToListAsync();
            return filterResult.FirstOrDefault();
        }

        public async Task AddAsync(T dto)
        {
            await Collection.InsertOneAsync(dto);
        }

        public async Task AddAsync(IEnumerable<T> listDto)
        {
            await Collection.InsertManyAsync(listDto);
        }

        public async Task<T> UpdateAsync(T dto)
        {
            var filter = Builders<T>.Filter.Eq("_id", dto.Id);
            return await Collection.FindOneAndReplaceAsync(filter, dto);
        }

        public async Task<T> DeleteAsync(T dto)
        {
            var filter = Builders<T>.Filter.Eq("_id", dto.Id);
            return await Collection.FindOneAndDeleteAsync(filter);
        }

        public async Task<List<T>> FilterAsync(BaseMdPagingFilter<T> mdFilter)
        {
            var filterSpecification = mdFilter.GenerateFilterSpecification();
            var findOptions = new FindOptions<T, T>();
            if (filterSpecification.Pagination != null)
            {
                findOptions.Skip = filterSpecification.Pagination.Skip;
                findOptions.Limit = filterSpecification.Pagination.Limit;
            }

            if (filterSpecification.Sort != null)
                findOptions.Sort = filterSpecification.Sort;

            var filterResult = await Collection.FindAsync(filterSpecification.Filter, findOptions);
            var result = await filterResult.ToListAsync();
            return result;
        }

        public async Task<int> CountAsync(BaseMdPagingFilter<T> mdFilter)
        {
            var filterSpecification = mdFilter.GenerateFilterSpecification();
            var countResult = await Collection.CountDocumentsAsync(filterSpecification.Filter);
            return (int) countResult;
        }
    }
}