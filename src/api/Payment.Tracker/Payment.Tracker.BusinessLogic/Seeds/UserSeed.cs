using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Service.Action.Core;
using Payment.Tracker.BusinessLogic.Configuration;
using Payment.Tracker.BusinessLogic.Services;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.BusinessLogic.Seeds
{
    public class UserSeed : ISeed
    {
        private const string AdminUserName = "admin";
        private readonly IGenericRepository<User> _usersRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ISecuritySettings _securitySettings;

        public UserSeed(
            IGenericRepository<User> usersRepository,
            IAuthenticationService authenticationService,
            ISecuritySettings securitySettings)
        {
            _usersRepository = usersRepository;
            _authenticationService = authenticationService;
            _securitySettings = securitySettings;
        }

        public async Task SeedAsync()
        {
            if (await _usersRepository.ExistsAsync(new Filter<User>(u => u.UserName == AdminUserName)))
            {
                return;
            }

            var user = new User
            {
                UserName = AdminUserName
            };

            IServiceActionResult result = _authenticationService.CreatePasswordHash(user, _securitySettings.AdminPassword);
            var errors = result.ErrorMessages?.ToList() ?? new List<string>();
            if (errors.Any())
            {
                throw new Exception($"Błąd seedowania bazy danych: {string.Join(". ", errors)}");
            }

            await _usersRepository.InsertAsync(user);
        }
    }
}