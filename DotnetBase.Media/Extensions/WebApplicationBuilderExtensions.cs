using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace DotnetBase.Media.Extensions
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Base .Net 8 Media Restfull", Version = "v1" });
            });
            services.AddCors();
            services.AddHttpContextAccessor();
        }

        public static void BuildAndRun(this WebApplicationBuilder builder)
        {
            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Base .Net 8 Media Restfull"));
            }

            app.UseCors(x => x
              .SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());

            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}
