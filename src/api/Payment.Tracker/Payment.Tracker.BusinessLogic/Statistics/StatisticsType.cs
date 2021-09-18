using System.ComponentModel.DataAnnotations;

namespace Payment.Tracker.BusinessLogic.Statistics
{
    public enum StatisticsType
    {
        [Display(Name = "Koszt całkowity na miesiąc")]
        TotalCostsPerMonth = 1,
    }
}