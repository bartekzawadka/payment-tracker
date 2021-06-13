using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.DataLayer.Repositories
{
    public interface IGenericRepository<TCollection> where TCollection : Document
    {
        Task<bool> ExistsAsync<TFilter>(TFilter filter) where TFilter : Filter<TCollection>, new();

        Task<long> CountAsync<TFilter>(TFilter filter) where TFilter : Filter<TCollection>, new();

        Task<List<TNew>> GetAllAsAsync<TFilter, TNew>(
            Expression<Func<TCollection, TNew>> selectClause,
            TFilter filter = (TFilter) null)
            where TFilter : Filter<TCollection>, new();

        Task<TCollection> GetByIdAsync(string id);

        Task<TOut> GetByIdAsAsync<TOut>(
            string id,
            Expression<Func<TCollection, TOut>> selectClause);

        Task<TCollection> GetOneAsync<TFilter>(TFilter filter) where TFilter : Filter<TCollection>, new();

        Task<TOut> GetOneAsAsync<TFilter, TOut>(
            TFilter filter,
            Expression<Func<TCollection, TOut>> selectClause)
            where TFilter : Filter<TCollection>, new();
        
        Task InsertAsync(TCollection item);

        Task InsertManyAsync(IEnumerable<TCollection> items);

        Task UpdateAsync(string id, TCollection item);

        Task DeleteAsync(string id);

        Task DeleteAsync(IEnumerable<string> ids);
    }
}