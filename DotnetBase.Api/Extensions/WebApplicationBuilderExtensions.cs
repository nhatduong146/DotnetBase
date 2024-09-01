using DotnetBase.Application.Services;
using DotnetBase.Application.Services.DataInitialize;
using DotnetBase.Domain.Entities;
using DotnetBase.Domain.Entities.Contexts;
using DotnetBase.Infrastructure.Caching;
using DotnetBase.Infrastructure.Common.Constants;
using DotnetBase.Infrastructure.Mvc.Filters;
using DotnetBase.Infrastructure.Mvc.Utilities;
using DotnetBase.Infrastructure.Services.DataInitialize;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotnetBase.Api.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static void BuildConfiguration(this WebApplicationBuilder builder)
        {
            var configurationManager = builder.Configuration;

            configurationManager
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(l => l.Level == Serilog.Events.LogEventLevel.Information)
                    .WriteTo.File("logs/informations/log-information-{Date}.txt",
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(l => l.Level == Serilog.Events.LogEventLevel.Warning)
                    .WriteTo.File("logs/warnings/log-warnings-{Date}.txt",
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(l => l.Level == Serilog.Events.LogEventLevel.Error)
                    .WriteTo.File("logs/errors/log-errors-{Date}.txt",
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
                .CreateLogger();
        }

        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;

            services.AddControllers(
                options =>
                {
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                    options.Filters.Add(typeof(ValidateModelFilter));
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                });
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Base .Net 8 Restful API - Mediator Design Pattern",
                    Description = "Base .Net 8 Restful API - Mediator Design Pattern",
                    TermsOfService = null,
                });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = JwtBearerDefaults.AuthenticationScheme,
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
                c.DescribeAllParametersInCamelCase();
            });

            var msSqlConnectionString = builder.Configuration.GetValue<string>("database:msSql:connectionString");
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(
                    msSqlConnectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure();
                    }));

            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:ThirdPartyRelationshipSecret"))),
                        ValidAudience = builder.Configuration.GetValue<string>("AppSettings:TokenAudience"),
                        ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:TokenIssuer"),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            // Add policy for each Role
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policy.USER_ACCESS,
                            policy => policy.RequireRole(RoleConstants.ADMIN, RoleConstants.USER));

                options.AddPolicy(Policy.ADMIN_ACCESS,
                            policy => policy.RequireRole(RoleConstants.ADMIN));

            });

            services.AddCors();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            // Health checks
            services.AddHealthChecks()
                .AddSqlServer(msSqlConnectionString);

            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains(GlobalConstants.ASSEMBLY_NAME)).ToArray();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(currentAssemblies));
            services.AddMapster();

            //Register MemoryCacheManager
            services.AddScoped<ICacheManager, MemoryCacheManager>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IDataInitializeService, RoleDataInitializeService>();
            services.AddTransient<IDataInitializeService, UserDataInitializeService>();
            services.AddTransient<IDataInitializeService, CategoryInitializeService>();

            ServiceLocator.SetLocatorProvider(services.BuildServiceProvider());
        }

        public static void BuildAndRun(this WebApplicationBuilder builder)
        {
            var app = builder.Build();
            var serviceProvider = builder.Services.BuildServiceProvider();

            var useSwagger = builder.Configuration.GetValue<bool>("AppSettings:UseSwagger");
            if (useSwagger)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Base .Net 8 Restful API - Mediator Design Pattern");
                    c.DocumentTitle = "Base .Net 8 Restful API - Document";
                    c.DocExpansion(DocExpansion.None);
                });
            }

            app.UseCors(x => x
               .SetIsOriginAllowed(origin => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            var db = serviceProvider.GetRequiredService<AppDbContext>();

            // Auto run migrate
            db.Database.MigrateAsync().Wait();

            // Get the service  
            var dataInitializeServices = serviceProvider.GetServices<IDataInitializeService>();
            foreach (var service in dataInitializeServices)
            {
                service.RunAsync().Wait();
            }

            app.Run();
        }
    }
}
