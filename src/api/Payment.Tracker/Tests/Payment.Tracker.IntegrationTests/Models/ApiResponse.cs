using System.Net;

namespace Payment.Tracker.IntegrationTests.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public ApiResponse(HttpStatusCode code, bool isSuccess)
        {
            StatusCode = code;
            IsSuccess = isSuccess;
        }
    }
}