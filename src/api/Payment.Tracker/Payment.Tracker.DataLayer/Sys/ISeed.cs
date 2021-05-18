using System.Threading.Tasks;

namespace Payment.Tracker.DataLayer.Sys
{
    public interface ISeed
    {
        Task SeedAsync();
    }
}