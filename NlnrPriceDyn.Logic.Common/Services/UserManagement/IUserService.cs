using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.Logic.Common.Models;

namespace NlnrPriceDyn.Logic.Common.Services
{
    public interface IUserService
    {
        Task<UserDB> FindByNameAsync(string userName);

        Task<UserDB> FindByIdAsync(string userId);

        Task<bool> CheckPasswordAsync(UserDB userDbToVerify, string password);

        Task<IdentityResult> RegisterUserAsync(UserRegistrationRequest request);

        Task<IdentityResult> ConfirmUserEmailAsync(string userId, string code);

        Task<bool> SendEmailConfirmationTokenAsync(string userId);

        Task<IList<string>> GetUserRolesAsync(string userId);
    }
}
