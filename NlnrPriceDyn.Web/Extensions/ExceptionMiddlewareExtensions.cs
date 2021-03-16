using Microsoft.AspNetCore.Builder;
using NlnrPriceDyn.Web.Helpers;

namespace NlnrPriceDyn.Web.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
