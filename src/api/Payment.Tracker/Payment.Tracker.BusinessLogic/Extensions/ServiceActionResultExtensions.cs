using System.Collections.Generic;
using System.Linq;
using Baz.Service.Action.Core;

namespace Payment.Tracker.BusinessLogic.Extensions
{
    public static class ServiceActionResultExtensions
    {
        public static bool IsSuccessful(this IServiceActionResult serviceActionResult)
        {
            var errorMessages = serviceActionResult.ErrorMessages?.ToList() ?? new List<string>();
            return errorMessages.Count == 0;
        }
    }
}