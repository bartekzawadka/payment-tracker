using System.Linq;

namespace Payment.Tracker.BusinessLogic.ServiceAction
{
    public class ServiceActionResult<T> : ServiceActionResult, IServiceActionResult<T>
    {
        private readonly T _data;

        private ServiceActionResult(ServiceActionResultType resultType, params string[] messages)
            : base(resultType, messages)
        {
        }

        private ServiceActionResult(T data) : base(ServiceActionResultType.Success)
        {
            ResultType = ServiceActionResultType.Success;
            _data = data;
        }

        private ServiceActionResult(ServiceActionResultType resultType, T data) : base(resultType)
        {
            _data = data;
        }

        public static ServiceActionResult<T> GetSuccess(T data) => new(data);

        public static ServiceActionResult<T> GetCreated(T data) => new(ServiceActionResultType.Created, data);

        public new static ServiceActionResult<T> GetDataError(params string[] messages) =>
            new(ServiceActionResultType.InvalidDataOrOperation, messages);

        public new static ServiceActionResult<T> GetNotFound(params string[] messages) =>
            new(ServiceActionResultType.ObjectNotFound, messages);

        public new static ServiceActionResult<T> GetUnauthorized(params string[] messages) =>
            new(ServiceActionResultType.UnauthorizedAccess, messages);

        public new static ServiceActionResult<T> GetForbidden(params string[] messages) =>
            new(ServiceActionResultType.ForbiddenAccess, messages);

        public T GetData() => _data;

        public IServiceActionResult<TOut> GetFailureAs<TOut>() =>
            new ServiceActionResult<TOut>(ResultType, ErrorMessages.ToArray());
    }
}