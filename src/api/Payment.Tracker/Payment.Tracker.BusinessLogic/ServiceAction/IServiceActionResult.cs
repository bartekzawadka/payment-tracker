using System.Collections.Generic;

namespace Payment.Tracker.BusinessLogic.ServiceAction
{
    public interface IServiceActionResult
    {
        ServiceActionResultType ResultType { get; }

        IEnumerable<string> ErrorMessages { get; }

        bool IsSuccess { get; }
    }
}