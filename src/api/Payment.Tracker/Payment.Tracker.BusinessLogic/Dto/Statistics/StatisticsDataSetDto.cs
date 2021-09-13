using System.Collections.Generic;

namespace Payment.Tracker.BusinessLogic.Dto.Statistics
{
    public class StatisticsDataSetDto<T>
    {
        public string Label { get; set; }

        public ICollection<T> Data { get; set; }
    }
}