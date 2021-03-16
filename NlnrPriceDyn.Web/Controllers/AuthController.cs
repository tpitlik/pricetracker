using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.Logic.Common.Models;
using NlnrPriceDyn.Logic.Common.Services;
using NlnrPriceDyn.Web.Auth;
using NlnrPriceDyn.Web.Helpers;
using NlnrPriceDyn.Web.Models;

namespace NlnrPriceDyn.Web.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IUserService _service;
        public readonly IConfiguration _configuration;

        public AuthController(IUserService service, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, IConfiguration configuration)
        {
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _service = service;
            _configuration = configuration;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> UserLogin([FromBody]UserLoginRequest credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // get the user to verifty
            var userToVerify = await _service.FindByNameAsync(credentials.UserName);

            var identity = await GetClaimsIdentity(userToVerify, credentials.UserName, credentials.Password);
            if (identity == null)
            {

                return BadRequest(Errors.AddErrorToModelState("login_error", Resources.ErrorLoginFailure, ModelState));
            }

            if (!userToVerify.EmailConfirmed)
            {
                string callbackUrl = _configuration.GetValue<string>("App:FrontendUrl");
                return BadRequest(Errors.AddErrorToModelState("email_error", String.Format(Resources.ErrorEmailNotConfirmed, callbackUrl, userToVerify.Id), ModelState));
            }

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return new OkObjectResult(jwt);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(UserDB userToVerify, string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _service.CheckPasswordAsync(userToVerify, password))
            {
                var userRolesList = await _service.GetUserRolesAsync(userToVerify.Id);
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id, userRolesList));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}