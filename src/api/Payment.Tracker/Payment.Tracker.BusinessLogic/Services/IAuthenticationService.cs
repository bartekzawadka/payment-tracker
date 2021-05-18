using System.Threading.Tasks;
using Payment.Tracker.BusinessLogic.Dto.Auth;
using Payment.Tracker.BusinessLogic.ServiceAction;
using Payment.Tracker.DataLayer.Models;

namespace Payment.Tracker.BusinessLogic.Services
{
    public interface IAuthenticationService
    {
        Task<IServiceActionResult<TokenDto>> AuthenticateAsync(string password);

        IServiceActionResult CreatePasswordHash(User user, string password);
    }
}