using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NlnrPriceDyn.Web.Helpers
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol", Id = "id";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
            }

            public static class ApiUserRoles
            {
                public const string GeneralUserRole = "GeneralUser";
                public const string DemoUserRole = "DemoUser";
                public const string AdminUserRole = "Admin";
            }

            public static class ApiPolicies
            {
                public const string DemoGeneralUserPolicy = "DemoGeneralUser";
                public const string GeneralUserPolicy = "GeneralUserOnly";
                public const string AdminUserPolicy = "AdminOnly";
            }
        }
    }
}
