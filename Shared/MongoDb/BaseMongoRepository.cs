using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ez.OrderServices.ReadSide.MQ.Domain;
using MongoDB.Driver;
using Shared.MongoDb.Filters;

namespace Shared.MongoDb
{
    public abstract class BaseRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoDatabase Database;

        protected BaseRepository(IMongoDbContext context)
        {
            Database = context.Database;
            Collection = Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public IMongoCollection<TEntity> Collection { get; }

        public async Task<List<TEntity>> FilterAsync(BaseMdPagingFilter<TEntity> mdFilter)
        {
            var filterSpecification = mdFilter.GenerateFilterSpecification();
            var findOptions = new FindOptions<TEntity, TEntity>();
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

        public async Task<int> CountAsync(BaseMdPagingFilter<TEntity> mdFilter)
        {
            var filterSpecification = mdFilter.GenerateFilterSpecification();
            var countResult = await Collection.CountDocumentsAsync(filterSpecification.Filter);
            return (int)countResult;
        }

        public virtual async Task<TEntity> AddAsync(TEntity obj)
        {
            await Collection.InsertOneAsync(obj);
            return obj;
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> objs)
        {
            await Collection.InsertManyAsync(objs);
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            var filterResult = await (await Collection.FindAsync(FilterId(id))).ToListAsync();
            return filterResult.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var all = await Collection.FindAsync(Builders<TEntity>.Filter.Empty);
            return all.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> filter)
        {
            var all = await Collection.FindAsync(filter);
            return all.ToList();
        }

        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter)
        {
            var all = await Collection.FindAsync(filter);
            return all.ToList().FirstOrDefault();
        }

        public virtual async Task<TEntity> UpdateAsync(int id, TEntity obj)
        {
            await Collection.ReplaceOneAsync(FilterId(id), obj);
            return obj;
        }

        public virtual async Task<bool> RemoveAsync(int id)
        {
            var result = await Collection.DeleteOneAsync(FilterId(id));
            return result.IsAcknowledged;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private static FilterDefinition<TEntity> FilterId(int key)
        {
            return Builders<TEntity>.Filter.Eq("Id", key);
        }
    }

    public class QueryResult<T>
    {
        public QueryResult()
        {
        }

        public QueryResult(long count, IEnumerable<T> items)
        {
            Count = count;
            Items = items;
        }

        public long Count { get; set; }
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        public static QueryResult<T> Empty()
        {
            return new QueryResult<T>(0, null);
        }
    }

    public static class QueryResultExtension
    {
        //public static async Task<QueryResult<T>> ToQueryResultAsync<T>(this IMongoQueryable<T> queryable, int pageIndex,
        //    int pageSize, Sorts sort) where T : class
        //{
        //    if (sort == null)
        //        return new QueryResult<T>
        //        {
        //            Count = await queryable.CountAsync(),
        //            Items = await queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync()
        //        };
        //    return new QueryResult<T>
        //    {
        //        Count = await queryable.CountAsync(),
        //        Items = await queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToSort(sort).ToListAsync()
        //    };
        //}

        public static QueryResult<K> Select<T, K>(this QueryResult<T> @this, Func<T, K> selector)
        {
            return new QueryResult<K>
            {
                Count = @this.Count,
                Items = @this.Items.Select(selector)
            };
        }
    }
}