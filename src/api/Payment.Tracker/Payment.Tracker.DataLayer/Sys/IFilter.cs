namespace Payment.Tracker.DataLayer.Sys
{
    public interface IFilter
    {
        int PageSize { get; }

        int PageIndex { get; }

        int PageNumber { get; }
    }
}