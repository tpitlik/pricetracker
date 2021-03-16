using System;
using Microsoft.AspNetCore.Identity;

namespace NlnrPriceDyn.DataAccess.Common.Models
{
    public class UserDB : IdentityUser
    {
        public DateTime RegistrationDate { get; set; }
    }
}
