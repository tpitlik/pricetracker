using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.DataAccess.Common.Repositories;
using NlnrPriceDyn.DataAccess.Contexts;

namespace NlnrPriceDyn.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<UserDB> _userManager;

        public UserRepository(UserManager<UserDB> userManager)
        {
            _userManager = userManager;
        }


        public Task<UserDB> FindByNameAsync(string userName)
        {
            return _userManager.FindByNameAsync(userName);
        }

        public Task<UserDB> FindByIdAsync(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        public Task<IList<string>> GetUserRoles(UserDB user)
        {
            return _userManager.GetRolesAsync(user);
        }

        public Task<bool> CheckPasswordAsync(UserDB userDb, string password)
        {
            return _userManager.CheckPasswordAsync(userDb, password);
        }

        public Task<IdentityResult> CreateAsync(UserDB userDb, string password)
        {
            var result = _userManager.CreateAsync(userDb, password);
            return result;
        }

        public Task<IdentityResult> AddToRoleAsync(UserDB userDb, string role)
        {
            var result = _userManager.AddToRoleAsync(userDb, role);
            return result;
        }

        public Task<IdentityResult> RemoveFromRoleAsync(UserDB userDb, string role)
        {
            var result = _userManager.RemoveFromRoleAsync(userDb, role);
            return result;
        }

        public Task<IList<string>> GetUserRolesAsync(UserDB userDb)
        {
            return _userManager.GetRolesAsync(userDb);
        }

            public Task<string> GetEmailConfirmationToken(UserDB userIdentity)
        {
            return _userManager.GenerateEmailConfirmationTokenAsync(userIdentity);
        }

        public Task<IdentityResult> ConfirmUserEmailAsync(UserDB userIdentity, string code)
        {
            return _userManager.ConfirmEmailAsync(userIdentity, code);
        }

        public Task<int> GetTotalUsersCount()
        {
            return _userManager.Users.CountAsync();
        }

        public Task<IQueryable<UserDB>> GetUsers() { 

                return Task.FromResult<IQueryable<UserDB>>(_userManager.Users);
        }

    }
}
