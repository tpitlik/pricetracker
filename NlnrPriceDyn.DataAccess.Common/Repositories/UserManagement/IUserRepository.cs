using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NlnrPriceDyn.DataAccess.Common.Models;

namespace NlnrPriceDyn.DataAccess.Common.Repositories
{
    public interface IUserRepository
    {
        Task<UserDB> FindByIdAsync(string userId);

        Task<UserDB> FindByNameAsync(string userName);

        Task<IdentityResult> AddToRoleAsync(UserDB userDb, string role);
        Task<IdentityResult> RemoveFromRoleAsync(UserDB userDb, string role);

        Task<IList<string>> GetUserRolesAsync(UserDB userDb);

        Task<IList<string>> GetUserRoles(UserDB user);

        Task<bool> CheckPasswordAsync(UserDB userDb, string password);

        Task<IdentityResult> CreateAsync(UserDB userDb, string password);

        Task<string> GetEmailConfirmationToken(UserDB userIdentity);

        Task<IdentityResult> ConfirmUserEmailAsync(UserDB userIdentity, string code);

        Task<int> GetTotalUsersCount();
        Task<IQueryable<UserDB>> GetUsers();
    }
}