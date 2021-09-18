namespace Payment.Tracker.BusinessLogic.Dto.Statistics
{
    public class StatisticsDataPoint<T>
    {
        public string X { get; set; }

        public T Y { get; set; }
    }
}