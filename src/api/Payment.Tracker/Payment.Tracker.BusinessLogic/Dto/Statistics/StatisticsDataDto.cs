using System.Collections.Generic;

namespace Payment.Tracker.BusinessLogic.Dto.Statistics
{
    public class StatisticsDataDto<T>
    {
        public ICollection<string> Labels { get; set; }
        
        public ICollection<StatisticsDataSetDto<T>> Datasets { get; set; }
    }
}