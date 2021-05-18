using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Payment.Tracker.DataLayer.Models;

namespace Payment.Tracker.DataLayer.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : Identifiable
    {
        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> filter);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter);

        Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeNavigations);
        
        List<TOut> GetAs<TOut>(
            Expression<Func<TEntity, TOut>> selectExpression,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeNavigations);

        Task<TEntity> GetByIdAsync(object id);

        Task<TEntity> GetByIdWithIncludesAsync(object id, params Expression<Func<TEntity, object>>[] includeNavigations);

        Task<List<TOut>> GetAsAsync<TOut>(
            Expression<Func<TEntity, TOut>> selectExpression,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeNavigations);

        // Task<PagedList<TOut>> GetPagedAsAsync<TOut>(
        //     int pageIndex,
        //     int pageSize,
        //     Expression<Func<TEntity, TOut>> selectExpression,
        //     Expression<Func<TEntity, bool>> filter = null,
        //     Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //     params Expression<Func<TEntity, object>>[] includeNavigations);

        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter);

        Task<TOut> GetOneAsAsync<TOut>(object id, Expression<Func<TEntity, TOut>> selectExpression);

        Task<TEntity> InsertAsync(TEntity entity);

        Task InsertManyAsync(IEnumerable<TEntity> entities);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        Task SaveChangesAsync();
    }
}