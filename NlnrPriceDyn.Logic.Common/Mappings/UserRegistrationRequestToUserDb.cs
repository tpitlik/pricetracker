using AutoMapper;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.Logic.Common.Models;

namespace NlnrPriceDyn.Logic.Common.Mappers
{
    public class UserRegistrationRequestToUserDb : Profile
    {
        public UserRegistrationRequestToUserDb()
        {
            CreateMap<UserRegistrationRequest, UserDB>();
        }
    }
}
