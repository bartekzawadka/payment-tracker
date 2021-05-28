using System.Net;

namespace Payment.Tracker.IntegrationTests.Models
{
    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; }

        public ApiResponse(HttpStatusCode statusCode, bool isSuccess, T data) : base(statusCode, isSuccess)
        {
            Data = data;
        }
    }
}