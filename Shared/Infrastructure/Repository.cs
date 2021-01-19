using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Sort;
using Shared.Dto;
using Shared.EF.Interfaces;
using Shared.Extensions;

namespace Shared.Infrastructure
{
    public class Repository<TDbContext, T> : IRepository<T> where T : class where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public Repository(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
        }

        public DbSet<T> GetDbSet()
        {
            return _dbContext.Set<T>();
        }

        public async Task<QueryResult<T>> QueryAsync(Expression<Func<T, bool>> predicate, int skip, int take,
            Sorts sort)
        {
            var queryable = _dbContext.Set<T>().Where(predicate);
            return await queryable.ToQueryResultAsync(skip, take, sort);
        }

        public async Task<IList<T>> GetManyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IList<T>> GetManyAsync(Expression<Func<T, bool>> predicate, int take)
        {
            return await _dbContext.Set<T>().Where(predicate).Take(take).ToListAsync();
        }

        public async Task<IList<T>> GetManyAsync(Expression<Func<T, bool>> predicate, int take,
            Expression<Func<T, object>> order = null)
        {
            var queryable = _dbContext.Set<T>().Where(predicate);
            if (order != null) queryable = queryable.OrderByDescending(order);

            return await queryable.Take(take).ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<TType> GetSingleAsync<TType>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TType>> select = null) where TType : class
        {
            if (select != null) return await _dbContext.Set<T>().Where(predicate).Select(select).FirstOrDefaultAsync();

            var entity = await _dbContext.Set<T>().Where(predicate).FirstOrDefaultAsync();
            return entity.To<TType>();
        }

        public async Task<T> GetByIdAsync(Guid entityId)
        {
            return await _dbContext.Set<T>().FindAsync(entityId);
        }

        public async Task<T> GetByIdAsync<TKey>(TKey entityId)
        {
            return await _dbContext.Set<T>().FindAsync(entityId);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AnyAsync(predicate);
        }

        public virtual T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            return entity;
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
        }

        public T Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }

        public T Delete(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
            return entity;
        }

        public TKey Delete<TKey>(TKey entityId)
        {
            var entity = _dbContext.Find<T>(entityId);
            _dbContext.Entry(entity).State = EntityState.Deleted;
            return entityId;
        }

        public virtual void DeleteMany(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public async Task<int> CountAllAsync()
        {
            return await _dbContext.Set<T>().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().CountAsync(predicate);
        }

        public async Task<T> GetRandom()
        {
            return await _dbContext.Set<T>().OrderBy(x => Guid.NewGuid()).FirstOrDefaultAsync();
        }

        public async Task<IList<TType>> GetAsync<TType>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TType>> select) where TType : class
        {
            return await _dbContext.Set<T>().Where(predicate).Select(select).ToListAsync();
        }

        public async Task<IList<TType>> GetAsync<TType>(Expression<Func<T, TType>> select) where TType : class
        {
            return await _dbContext.Set<T>().Select(select).ToListAsync();
        }

        public async Task<IList<T>> GetManyAsync(Expression<Func<T, bool>> predicate, int skip, int take)
        {
            return await _dbContext.Set<T>().Where(predicate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<T> GetSingleAsync()
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync();
        }
    }
}