using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;
using Payment.Tracker.DataLayer.Models;

namespace Payment.Tracker.DataLayer.Sys
{
    public class Filter<T> : IFilter where T : Document
    {
        private int _pageSize = 10;
        private int _pageIndex;

        public Filter()
        {
        }

        public Filter(Expression<Func<T, bool>> expression)
        {
            FilterDefinition = new ExpressionFilterDefinition<T>(expression);
        }
        
        public FilterDefinition<T> FilterDefinition { get; set; } = FilterDefinition<T>.Empty;

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                {
                    value = 10;
                }

                _pageSize = value;
            }
        }

        public virtual List<ColumnSort> Sorting { get; set; } = new()
        {
            new ColumnSort
            {
                ColumnName = nameof(Document.Id),
                IsDescending = false
            }
        };

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                if (value <= 0)
                {
                    value = 0;
                }

                _pageIndex = value;
            }
        }

        public int PageNumber => PageIndex + 1;
    }
}