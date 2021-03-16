using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.Logic.Common.Models;
using NlnrPriceDyn.Logic.Common.Services;
using NUnit.Framework;
using NlnrPriceDyn.Web;
using NlnrPriceDyn.Web.Auth;
using NlnrPriceDyn.Web.Controllers;
using NlnrPriceDyn.Web.Models;

namespace NlnrPriceDyn.Tests
{
    public class AuthControllerTests
    {
        public UserDB userIdentity;
        public Mock<IUserService> userServiceMock;
        public Mock<JwtFactory> jwtFactoryMock;
        public Mock<IConfiguration> configurationMock;
        public IOptions<JwtIssuerOptions> jwtOptions;


        [SetUp]
        public void Setup()
        {
            userIdentity = new UserDB()
            {
                Id = "uid-0000-001",
                Email = "test@mail.com",
                EmailConfirmed = false,
                UserName = "test user"
            };

            userServiceMock = new Mock<IUserService>();
            jwtOptions = Options.Create<JwtIssuerOptions>(
                new JwtIssuerOptions()
                {
                    Issuer = "Issuer",
                    Audience = "Audience",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("security_test_key_123456789")),
                                             SecurityAlgorithms.HmacSha256)
                });

            jwtFactoryMock = new Mock<JwtFactory>(jwtOptions);
            configurationMock = new Mock<IConfiguration>();

        }

        [Test]
        public async Task AuthControllerUserLoginExistingUserNotConfirmedEmail()
        {
            userServiceMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(userIdentity));
            userServiceMock.Setup(x => x.CheckPasswordAsync(It.IsAny<UserDB>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            configurationMock.Setup(c => c.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

            var authController = new AuthController(userServiceMock.Object, jwtFactoryMock.Object, jwtOptions, configurationMock.Object);
            var credentials = new UserLoginRequest()
            {
                UserName = "uname001",
                Password = "password"
            };
            var result = await authController.UserLogin(credentials) as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task AuthControllerUserLoginExistingUserConfirmedEmail()
        {
            userIdentity.EmailConfirmed = true;
            userServiceMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(userIdentity));
            userServiceMock.Setup(x => x.CheckPasswordAsync(It.IsAny<UserDB>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            configurationMock.Setup(c => c.GetSection(It.IsAny<String>())).Returns(new Mock<IConfigurationSection>().Object);

            var authController = new AuthController(userServiceMock.Object, jwtFactoryMock.Object, jwtOptions, configurationMock.Object);
            var credentials = new UserLoginRequest()
            {
                UserName = "uname001",
                Password = "password"
            };
            var result = await authController.UserLogin(credentials) as OkObjectResult;
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task AuthControllerUserLoginModelError()
        {
            var authController = new AuthController(userServiceMock.Object, jwtFactoryMock.Object, jwtOptions, configurationMock.Object);

            var credentials = new UserLoginRequest()
            {
                UserName = "",
                Password = "password"
            };

            authController.ModelState.AddModelError("usr_name_error", "User name is empty.");
            
            var result = await authController.UserLogin(credentials) as BadRequestObjectResult;

            Assert.NotNull(result);
        }
    }
}
