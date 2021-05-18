using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.DataLayer.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : Identifiable
    {
        private PaymentContext Context { get; }
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(PaymentContext context)
        {
            Context = context;
            _dbSet = Context.Set<TEntity>();
        }

        public Task<bool> ExistAsync(Expression<Func<TEntity, bool>> filter) => _dbSet.AnyAsync(filter);

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> filter) => _dbSet.CountAsync(filter);

        public List<TOut> GetAs<TOut>(
            Expression<Func<TEntity, TOut>> selectExpression,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeNavigations) =>
            GetQueryable(selectExpression, filter, orderBy, includeNavigations).ToList();

        public Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeNavigations) =>
            GetQueryable(filter, orderBy, includeNavigations).ToListAsync();

        public Task<TEntity> GetByIdAsync(object id) => GetByIdWithIncludesAsync(id, null);

        public Task<TEntity> GetByIdWithIncludesAsync(object id, params Expression<Func<TEntity, object>>[] includeNavigations)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
            Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.PropertyOrField(parameter, Consts.IdPropertyName),
                    Expression.Constant(id)),
                parameter);

            IQueryable<TEntity> query = _dbSet;

            if (includeNavigations != null && includeNavigations.Any())
            {
                query = includeNavigations.Aggregate(query, (current, expression) => current.Include(expression));
            }

            return query.SingleOrDefaultAsync(predicate);
        }

        public Task<List<TOut>> GetAsAsync<TOut>(
            Expression<Func<TEntity, TOut>> selectExpression,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeNavigations) =>
            GetQueryable(selectExpression, filter, orderBy, includeNavigations).ToListAsync();

        // public Task<PagedList<TOut>> GetPagedAsAsync<TOut>(
        //     int pageIndex,
        //     int pageSize,
        //     Expression<Func<TEntity, TOut>> selectExpression,
        //     Expression<Func<TEntity, bool>> filter = null,
        //     Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //     params Expression<Func<TEntity, object>>[] includeNavigations) =>
        //     GetQueryable(selectExpression, filter, orderBy, includeNavigations).ToPagedListAsync(pageIndex, pageSize);

        public Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter) =>
            _dbSet.SingleOrDefaultAsync(filter);

        private IQueryable<TEntity> GetQueryable(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeNavigations)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeNavigations != null && includeNavigations.Any())
            {
                query = includeNavigations.Aggregate(query, (current, expression) => current.Include(expression));
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query;
        }
        
        private IQueryable<TOut> GetQueryable<TOut>(
            Expression<Func<TEntity, TOut>> selectExpression,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeNavigations) =>
            GetQueryable(filter, orderBy, includeNavigations).Select(selectExpression);

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public Task InsertManyAsync(IEnumerable<TEntity> entities) => _dbSet.AddRangeAsync(entities);

        public void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }

            _dbSet.Remove(entityToDelete);
        }

        public Task SaveChangesAsync() => Context.SaveChangesAsync();
    }
}