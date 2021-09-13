using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.DataLayer.Repositories
{
    public class GenericRepository<TCollection> : IGenericRepository<TCollection> where TCollection : Document
    {
        protected IMongoCollection<TCollection> Collection { get; }
        
        public GenericRepository(PaymentContext context)
        {
            var name = Regex.Replace(typeof(TCollection).Name, "(\\B[A-Z])", ".$1").ToLower();
            Collection = context.Database.GetCollection<TCollection>(name);
        }
        
        public async Task<bool> ExistsAsync<TFilter>(TFilter filter) where TFilter : Filter<TCollection>, new() => 
            (await CountAsync(filter)) > 0;

        public async Task<long> CountAsync<TFilter>(TFilter filter) where TFilter : Filter<TCollection>, new()
        {
            filter ??= new TFilter();
            
            var count = await Collection.CountDocumentsAsync(filter.FilterDefinition);
            return count;
        }

        public Task<List<TCollection>> GetAllAsync<TFilter>(TFilter filter = null)
            where TFilter : Filter<TCollection>, new() =>
            GetFindFluentAs(item => item, filter).ToListAsync();

        public Task<List<TNew>> GetAllAsAsync<TFilter, TNew>(
            Expression<Func<TCollection, TNew>> selectClause,
            TFilter filter = null)
            where TFilter : Filter<TCollection>, new() => GetFindFluentAs(selectClause, filter).ToListAsync();

        private IFindFluent<TCollection, TNew> GetFindFluentAs<TFilter, TNew>(
            Expression<Func<TCollection, TNew>> selectClause,
            TFilter filter = null)
            where TFilter : Filter<TCollection>, new()
        {
            filter ??= new TFilter();

            var query = Collection.Find(filter.FilterDefinition);

            if (filter.Sorting?.Count > 0)
            {
                var builder = new SortDefinitionBuilder<TCollection>();
                foreach (var columnSort in filter.Sorting)
                {
                    var stringFieldDefinition = new StringFieldDefinition<TCollection>(columnSort.ColumnName);
                    Func<SortDefinition<TCollection>> sortDefinitionFunc;
                    if (columnSort.IsDescending)
                    {
                        sortDefinitionFunc = () => builder.Descending(stringFieldDefinition);
                    }
                    else
                    {
                        sortDefinitionFunc = () => builder.Ascending(stringFieldDefinition);
                    }

                    query = query.Sort(sortDefinitionFunc());
                }
            }

            var resultQuery = query.Project(selectClause);
            return resultQuery;
        }

        public async Task<TCollection> GetByIdAsync(string id)
        {
            var result = await Collection
                .FindAsync(document => string.Equals(document.Id, id));
            return await result.SingleOrDefaultAsync();
        }

        public Task<TOut> GetByIdAsAsync<TOut>(
            string id,
            Expression<Func<TCollection, TOut>> selectClause) =>
            GetFindFluentAs(selectClause, new Filter<TCollection>(x=>x.Id == id))
                .SingleOrDefaultAsync();

        public async Task<TCollection> GetOneAsync<TFilter>(TFilter filter) where TFilter : Filter<TCollection>, new()
        {
            var result = await Collection.FindAsync(filter.FilterDefinition);
            return await result.SingleOrDefaultAsync();
        }

        public Task<TOut> GetOneAsAsync<TFilter, TOut>(
            TFilter filter,
            Expression<Func<TCollection, TOut>> selectClause)
            where TFilter : Filter<TCollection>, new() =>
            GetFindFluentAs(selectClause, filter).SingleOrDefaultAsync();

        public Task InsertAsync(TCollection item) => Collection.InsertOneAsync(item);

        public Task InsertManyAsync(IEnumerable<TCollection> items) => Collection.InsertManyAsync(items);

        public Task UpdateAsync(string id, TCollection item) => Collection.ReplaceOneAsync(
            document => string.Equals(document.Id, id),
            item);

        public Task DeleteAsync(string id) =>
            Collection.DeleteOneAsync(document => string.Equals(document.Id, id));

        public Task DeleteAsync(IEnumerable<string> ids)
        {
            var filter = new FilterDefinitionBuilder<TCollection>().In(document => document.Id, ids);
            return Collection.DeleteManyAsync(filter);
        }
    }
}