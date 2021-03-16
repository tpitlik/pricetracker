using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.DataAccess.Common.Repositories;
using NlnrPriceDyn.Logic.Common.Models;
using NlnrPriceDyn.Logic.Common.Services.Messaging;
using NlnrPriceDyn.Logic.Services;
using NlnrPriceDyn.Logic.Common.Mappings;
using NUnit.Framework;
using NlnrPriceDyn.Logic.Common.Mappers;
using NlnrPriceDyn.Logic.Common.Models.Messaging;

namespace NlnrPriceDyn.Tests
{
    public class UserServiceTests
    {
        public IMapper mapper;
        public Mock<IMailService> mailerMock;
        public Mock<IUserRepository> repositoryMock;
        public Mock<IConfiguration> configurationMock;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new Mock<IUserRepository>();
            var profileUserRegistrationRequestToUserDb = new UserRegistrationRequestToUserDb();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(profileUserRegistrationRequestToUserDb);
            });
            mapper = new Mapper(configuration);
            mailerMock = new Mock<IMailService>();
            configurationMock = new Mock<IConfiguration>();
        }

        [Test]
        public async Task UserServiceRegisterUserAsyncMaxUsersCountNotReached()
        {
            repositoryMock.Setup(x => x.GetTotalUsersCount()).Returns(Task.FromResult<int>(100));
            repositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserDB>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            var oneSectionMock = new Mock<IConfigurationSection>();
            oneSectionMock.Setup(s => s.Value).Returns("1000");
            configurationMock.Setup(c => c.GetSection(It.IsAny<string>())).Returns(oneSectionMock.Object);

            var request = new UserRegistrationRequest()
            {
                Email = "test@mail.com",
                Password = "password",
                PasswordConfirmation = "password"
            };

            var userService = new UserService(repositoryMock.Object, mapper, mailerMock.Object, configurationMock.Object);
            var result = await userService.RegisterUserAsync(request);
            Assert.AreEqual(true, result.Succeeded);
            mailerMock.Verify(x => x.SendAsync(It.IsAny<EmailMessage>()), Times.Once);
        }

        [Test]
        public async Task UserServiceRegisterUserAsyncMaxUsersCountReached()
        {
            repositoryMock.Setup(x => x.GetTotalUsersCount()).Returns(Task.FromResult<int>(100));
            repositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserDB>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            var oneSectionMock = new Mock<IConfigurationSection>();
            oneSectionMock.Setup(s => s.Value).Returns("99");
            configurationMock.Setup(c => c.GetSection(It.IsAny<string>())).Returns(oneSectionMock.Object);

            var request = new UserRegistrationRequest()
            {
                Email = "test@mail.com",
                Password = "password",
                PasswordConfirmation = "password"
            };

            var userService = new UserService(repositoryMock.Object, mapper, mailerMock.Object, configurationMock.Object);
            var result = await userService.RegisterUserAsync(request);
            mailerMock.Verify(x => x.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
            Assert.AreEqual(false, result.Succeeded);
        }
    }
}
