using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using Payment.Tracker.DataLayer.Models;

namespace Payment.Tracker.DataLayer.Sys
{
    public class FilterBuilder<T> where T : Document
    {
        private readonly Filter<T> _filter;
        private readonly List<FilterDefinition<T>> _filterDefinitions;

        private FilterBuilder()
        {
            _filter = new Filter<T>();
            _filterDefinitions = new List<FilterDefinition<T>>();
        }

        public static FilterBuilder<T> Create() => new();

        public FilterBuilder<T> WithAndFilterExpression(params Expression<Func<T, bool>>[] expressions)
        {
            AddFilterDefinitions(e => Builders<T>.Filter.And(e), expressions);
            return this;
        }

        public FilterBuilder<T> WithOrFilterExpression(params Expression<Func<T, bool>>[] expressions)
        {
            AddFilterDefinitions(e => Builders<T>.Filter.Or(e), expressions);
            return this;
        }

        private void AddFilterDefinitions(Func<FilterDefinition<T>, FilterDefinition<T>> func, params Expression<Func<T, bool>>[] expressions)
        {
            var definitions = expressions
                .Select(expression => func(new ExpressionFilterDefinition<T>(expression)))
                .ToList();

            _filterDefinitions.AddRange(definitions);
        }

        public FilterBuilder<T> WithPageSize(int pageSize)
        {
            _filter.PageSize = pageSize;
            return this;
        }
        
        public FilterBuilder<T> WithPageIndex(int pageIndex)
        {
            _filter.PageIndex = pageIndex;
            return this;
        }
        
        public FilterBuilder<T> WithSorting(List<ColumnSort> sorting)
        {
            _filter.Sorting = sorting;
            return this;
        }

        public Filter<T> Build()
        {
            var fDefBuilder = new FilterDefinitionBuilder<T>();
            _filter.FilterDefinition = _filterDefinitions.Count > 0
                ? fDefBuilder.And(_filterDefinitions)
                : FilterDefinition<T>.Empty;
            return _filter;
        }
    }
}