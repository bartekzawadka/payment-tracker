using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Payment.Tracker.BusinessLogic.Configuration;
using Payment.Tracker.BusinessLogic.Dto.Auth;
using Payment.Tracker.BusinessLogic.ServiceAction;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.BusinessLogic.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGenericRepository<User> _usersRepository;
        private readonly ISecuritySettings _securitySettings;

        public AuthenticationService(IGenericRepository<User> usersRepository, ISecuritySettings securitySettings)
        {
            _usersRepository = usersRepository;
            _securitySettings = securitySettings;
        }

        public async Task<IServiceActionResult<TokenDto>> AuthenticateAsync(string password)
        {
            User user = await _usersRepository.GetOneAsync(new Filter<User>(u => u.UserName == "admin"));
            if (user == null)
            {
                return ServiceActionResult<TokenDto>.GetUnauthorized("Nie odnaleziono użytkownika administratora");
            }

            return !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)
                ? ServiceActionResult<TokenDto>.GetUnauthorized("Niepoprawne hasło")
                : ServiceActionResult<TokenDto>.GetSuccess(GetApiToken(user));
        }

        public IServiceActionResult CreatePasswordHash(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return ServiceActionResult.GetDataError("Hasło nie może być puste");
            }

            if (user == null)
            {
                return ServiceActionResult.GetSuccess();
            }

            using var hmac = new HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return ServiceActionResult.GetSuccess();
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash == null || storedHash.Length != 64 || storedSalt == null || storedSalt.Length != 128)
            {
                throw new SystemException("Zaszyfrowane hasło jest w nieprawidłowym formacie");
            }

            using var hmac = new HMACSHA512(storedSalt);
            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return !computedHash.Where((t, i) => t != storedHash[i]).Any();
        }

        private TokenDto GetApiToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_securitySettings.TokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return new TokenDto
            {
                UserName = user.UserName,
                Token = tokenString
            };
        }
    }
}