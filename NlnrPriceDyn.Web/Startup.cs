using System;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.DataAccess.Common.Repositories;
using NlnrPriceDyn.DataAccess.Common.Repositories.ProductManagement;
using NlnrPriceDyn.Web.Auth;
using NlnrPriceDyn.DataAccess.Contexts;
using NlnrPriceDyn.DataAccess.Repositories;
using NlnrPriceDyn.Logic.Services;
using NlnrPriceDyn.Logic.Common.Models;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Services;
using NlnrPriceDyn.Logic.Common.Services.Messaging;
using NlnrPriceDyn.Web.Models;
using NlnrPriceDyn.Logic.Common.Validators;
using NlnrPriceDyn.Web.Extensions;
using NlnrPriceDyn.Web.Helpers;
using Swashbuckle.AspNetCore.Swagger;
using System.Threading.Tasks;

namespace NlnrPriceDyn.Web
{
    public class Startup
    {
        private const string SecretKey = "iNivDmHLpBY127sQsfhqGbRMdRj1PVkH";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("NlnrPriceDyn.DataAccess")));

            services.AddAutoMapper();

            services.AddIdentity<UserDB, IdentityRole>(
                    options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequiredLength = 6;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<RussianIdentityErrorDescriber>();

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = false,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = false,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("DemoGeneralUser", policy => policy.RequireRole(Constants.Strings.ApiUserRoles.GeneralUserRole, Constants.Strings.ApiUserRoles.DemoUserRole));
                options.AddPolicy(Constants.Strings.ApiPolicies.DemoGeneralUserPolicy, policy => policy.RequireRole(Constants.Strings.ApiUserRoles.GeneralUserRole, Constants.Strings.ApiUserRoles.DemoUserRole));
                //options.AddPolicy("GeneralUser", policy => policy.RequireRole(Constants.Strings.ApiUserRoles.GeneralUserRole));
                options.AddPolicy(Constants.Strings.ApiPolicies.GeneralUserPolicy, policy => policy.RequireRole(Constants.Strings.ApiUserRoles.GeneralUserRole));
                //options.AddPolicy("AdminOnly", policy => policy.RequireRole(Constants.Strings.ApiUserRoles.AdminUserRole));
                options.AddPolicy(Constants.Strings.ApiPolicies.AdminUserPolicy, policy => policy.RequireRole(Constants.Strings.ApiUserRoles.AdminUserRole));

            });

            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddTransient<IValidator<UserRegistrationRequest>, UserRegistrationRequestValidator>();
            services.AddTransient<IValidator<CreateUpdateProductRequest>, CreateUpdateProductRequestValidator>();
            services.AddTransient<IValidator<CreateUpdateProductNotificationRequest>, CreateUpdateProductNotificationRequestValidator>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IMailService, MailService>();
            services.AddSingleton(Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder =>
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials());
            });

            //services.AddMvc(options => options.OutputFormatters.RemoveType<StringOutputFormatter>());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Nlnr Price Dyn API", Version = "v1" });
            });

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            AddUpdatePricesJob();

            var cultureInfo = new CultureInfo("ru-Ru");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
                
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                //scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                CreateRoles(roleManager).Wait();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAnyOrigin");

            app.UseAuthentication();

            //app.UseSwagger();
            //app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nlnr Price Dyn API V1"); });

            app.UseHangfireServer();
            //app.UseHangfireDashboard();

            app.ConfigureCustomExceptionMiddleware();

            app.UseMvc();

        }

        private void AddUpdatePricesJob()
        {
            JobStorage.Current = new SqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));

            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            RecurringJob.AddOrUpdate<IProductService>((service) => service.UpdateProductPrices(), Cron.MinuteInterval(30));
        }

        private async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = {
                Constants.Strings.ApiUserRoles.AdminUserRole,
                Constants.Strings.ApiUserRoles.GeneralUserRole,
                Constants.Strings.ApiUserRoles.DemoUserRole
            };

            IdentityResult roleResult;
            
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

    }

}
