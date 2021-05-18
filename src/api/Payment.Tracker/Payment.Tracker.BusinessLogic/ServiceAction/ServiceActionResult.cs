using System.Collections.Generic;

namespace Payment.Tracker.BusinessLogic.ServiceAction
{
    public class ServiceActionResult : IServiceActionResult
    {
        public ServiceActionResultType ResultType { get; protected set; }

        public IEnumerable<string> ErrorMessages { get; }

        public bool IsSuccess => ResultType is < ServiceActionResultType.InvalidDataOrOperation and > ServiceActionResultType.None;

        protected ServiceActionResult(ServiceActionResultType resultType, params string[] messages)
        {
            ResultType = resultType;
            ErrorMessages = messages;
        }

        public static ServiceActionResult GetSuccess() => new(ServiceActionResultType.Success);

        public static ServiceActionResult GetDataError(params string[] messages) =>
            new(ServiceActionResultType.InvalidDataOrOperation, messages);

        public static ServiceActionResult GetNotFound(params string[] messages) =>
            new(ServiceActionResultType.ObjectNotFound, messages);

        public static ServiceActionResult GetUnauthorized(params string[] messages) =>
            new(ServiceActionResultType.UnauthorizedAccess, messages);

        public static ServiceActionResult GetForbidden(params string[] messages) =>
            new(ServiceActionResultType.ForbiddenAccess, messages);
    }
}