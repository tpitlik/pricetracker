using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.DataAccess.Common.Repositories;
using NlnrPriceDyn.Logic.Common.Exceptions;
using NlnrPriceDyn.Logic.Common.Models;
using NlnrPriceDyn.Logic.Common.Models.Messaging;
using NlnrPriceDyn.Logic.Common.Services;
using NlnrPriceDyn.Logic.Common.Services.Messaging;

namespace NlnrPriceDyn.Logic.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMailService _mailer;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository repository, IMapper mapper, IMailService mailer, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _mailer = mailer;
            _configuration = configuration;
        }

        public Task<UserDB> FindByNameAsync(string userName)
        {
            return _repository.FindByNameAsync(userName);
        }

        public Task<UserDB> FindByIdAsync(string userId)
        {
            return _repository.FindByIdAsync(userId);
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _repository.GetUserRoles(user);
                return result;
            }
            return new List<string>();
        }

        public Task<bool> CheckPasswordAsync(UserDB userDbToVerify, string password)
        {
            return _repository.CheckPasswordAsync(userDbToVerify, password);
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationRequest request)
        {
            IdentityResult registrationResult;
            int totalUsersCount = await _repository.GetTotalUsersCount();
            int maxUsersCount = _configuration.GetValue<int>("App:MaxUsers");
            if (totalUsersCount < maxUsersCount)
            {
                var userIdentity = _mapper.Map<UserDB>(request);
                userIdentity.RegistrationDate = DateTime.UtcNow;
                registrationResult = await _repository.CreateAsync(userIdentity, request.Password);
                if (registrationResult.Succeeded)
                {
                    var newUser = await _repository.FindByNameAsync(userIdentity.UserName);
                    await _repository.AddToRoleAsync(newUser, "GeneralUser");
                    //await _repository.AddToRoleAsync(newUser, "DemoUser");
                    await SendEmailConfirmationMessageAsync(newUser);
                }
            }
            else
            {
                registrationResult = IdentityResult.Failed(new IdentityError()
                    {Code = Resource.MaxUsersErrCode, Description = Resource.MaxUsersErrMsg});
            }

            return registrationResult;
        }

        public async Task<IdentityResult> ConfirmUserEmailAsync(string userId, string code)
        {
            var userIdentity = await _repository.FindByIdAsync(userId);
            return await _repository.ConfirmUserEmailAsync(userIdentity, code);
        }

        public async Task<bool> SendEmailConfirmationTokenAsync(string userId)
        {
            var userIdentity = await FindByIdAsync(userId);
            if (userIdentity == null)
            {
                return false;
            }
            await SendEmailConfirmationMessageAsync(userIdentity);
            return true;
        }

        private async Task SendEmailConfirmationMessageAsync(UserDB userIdentity)
        {
            string emailConfirmationCode = await _repository.GetEmailConfirmationToken(userIdentity);
            string callbackUrl = _configuration.GetValue<string>("App:FrontendUrl") + String.Format(Resource.EmailConfirmCallbackUrl, userIdentity.Id, HttpUtility.UrlEncode(emailConfirmationCode));
            var message = new EmailMessage()
            {
                Body = String.Format(Resource.EmailConfirmMsgBody, callbackUrl),
                Destination = userIdentity.Email,
                Subject = Resource.EmailConfirmMsgSubject
            };
            await _mailer.SendAsync(message);
        }
    }
}
