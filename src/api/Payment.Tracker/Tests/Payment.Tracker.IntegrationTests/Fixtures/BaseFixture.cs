using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Payment.Tracker.BusinessLogic.Dto.Auth;
using Payment.Tracker.BusinessLogic.Dto.User;

namespace Payment.Tracker.IntegrationTests.Fixtures
{
    public abstract class BaseFixture
    {
        private string _token;
        
        public async Task<string> GetTokenAsync(PaymentTrackerAppFactory factory)
        {
            if (!string.IsNullOrWhiteSpace(_token))
            {
                return _token;
            }

            using HttpClient client = factory.CreateClient();
            var logon = new LoginDto
            {
                Password = "Test1234"
            };

            HttpResponseMessage result = await client.PostAsync(
                "/user/authenticate",
                new StringContent(JsonConvert.SerializeObject(logon), Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode)
            {
                throw new AuthenticationException("Unable to get API token");
            }

            string responseDataString = await result.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenDto>(responseDataString);
            _token = token.Token;

            return _token;
        }
    }
}