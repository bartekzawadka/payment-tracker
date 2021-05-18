using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Tracker.BusinessLogic.Dto.Auth;
using Payment.Tracker.BusinessLogic.Dto.User;
using Payment.Tracker.BusinessLogic.ServiceAction;
using Payment.Tracker.BusinessLogic.Services;

namespace Payment.Tracker.Api.Controllers
{
    [Authorize]
    public class UserController : PaymentTrackerController
    {
        private readonly IAuthenticationService _authenticationService;

        public UserController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public Task<IServiceActionResult<TokenDto>> AuthenticateAsync([FromBody] LoginDto dto) =>
            _authenticationService.AuthenticateAsync(dto.Password);
    }
}