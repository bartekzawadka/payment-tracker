using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Payment.Tracker.IntegrationTests.Fixtures;
using Payment.Tracker.IntegrationTests.Models;

namespace Payment.Tracker.IntegrationTests.Tests
{
    public abstract class ApiTests<T> where T : BaseFixture
    {
        private readonly PaymentTrackerAppFactory _factory;

        protected abstract string ControllerName { get; }
        
        protected readonly T ApiCallFixture;
        
        protected ApiTests(PaymentTrackerAppFactory factory, T apiCallFixture)
        {
            _factory = factory;
            ApiCallFixture = apiCallFixture;
        }
        
        private string GetTargetUrl(string controllerMethodRouteUrlPart)
            => GetTargetUrl(ControllerName, controllerMethodRouteUrlPart);

        private static string GetTargetUrl(string controllerName, string controllerMethodRouteUrlPart)
            => $"/{controllerName.ToLower()}/{controllerMethodRouteUrlPart}";

        private async Task<HttpClient> GetClientAsync(string authToken = null)
        {
            HttpClient client = _factory.CreateClient();
            string token = !string.IsNullOrWhiteSpace(authToken)
                ? authToken
                : await ApiCallFixture.GetTokenAsync(_factory);
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Authorization = authHeader;
            return client;
        }

        protected async Task<ApiResponse> CallAsync(
            HttpMethod method,
            string actionUrlPart,
            bool useAuth,
            bool ensureSuccess)
        {
            HttpClient client = useAuth ? await GetClientAsync() : _factory.CreateClient();
            HttpRequestMessage message = await GetRequestMessageAsync(method, GetTargetUrl(actionUrlPart));
            HttpResponseMessage response = await client.SendAsync(message);
            
            return PostCallProcess(response, ensureSuccess);
        }
        
        protected async Task<ApiResponse> CallAsync<TIn>(
            HttpMethod method,
            string actionUrlPart,
            bool useAuthentication,
            bool ensureSuccess,
            TIn data)
        {
            HttpClient client = useAuthentication ? await GetClientAsync() : _factory.CreateClient();
            HttpRequestMessage message = await GetRequestMessageAsync(method, GetTargetUrl(actionUrlPart), data);
            HttpResponseMessage response = await client.SendAsync(message);

            return PostCallProcess(response, ensureSuccess);
        }
        
        protected async Task<ApiResponse<TOut>> CallAsync<TOut>(
            HttpMethod method,
            string actionUrlPart,
            bool useAuthentication,
            bool ensureSuccess)
        {
            HttpClient client = useAuthentication ? await GetClientAsync() : _factory.CreateClient();
            HttpRequestMessage message = await GetRequestMessageAsync(method, GetTargetUrl(actionUrlPart));
            HttpResponseMessage response = await client.SendAsync(message);

            return await PostCallProcessAsync<TOut>(response, ensureSuccess);
        }
        
        protected async Task<ApiResponse<TOut>> CallAsync<TIn, TOut>(
            HttpMethod method,
            string actionUrlPart,
            bool useAuthentication,
            bool ensureSuccess,
            TIn data)
        {
            HttpClient client = useAuthentication ? await GetClientAsync() : _factory.CreateClient();
            HttpRequestMessage message = await GetRequestMessageAsync(method, GetTargetUrl(actionUrlPart), data);
            HttpResponseMessage response = await client.SendAsync(message);

            return await PostCallProcessAsync<TOut>(response, ensureSuccess);
        }
        
        private async Task<HttpRequestMessage> GetRequestMessageAsync(HttpMethod method, string url)
        {
            var message = new HttpRequestMessage(method, url);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await ApiCallFixture.GetTokenAsync(_factory));
            return message;
        }

        private async Task<HttpRequestMessage> GetRequestMessageAsync<T>(HttpMethod method, string url, T data)
        {
            HttpRequestMessage message = await GetRequestMessageAsync(method, url);
            message.Content = GetStringContent(data);

            return message;
        }
        
        private static StringContent GetStringContent<T>(T data) =>
            new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        
        private static ApiResponse PostCallProcess(HttpResponseMessage response, bool ensureSuccess)
        {
            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }

            return new ApiResponse(response.StatusCode, response.IsSuccessStatusCode);
        }
        
        private static async Task<ApiResponse<TOut>> PostCallProcessAsync<TOut>(HttpResponseMessage response, bool ensureSuccess)
        {
            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }

            string resultString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TOut>(resultString);
            return new ApiResponse<TOut>(response.StatusCode, response.IsSuccessStatusCode, result);
        }
    }
}