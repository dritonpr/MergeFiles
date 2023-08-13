using AspNetCoreRateLimit;
using MergeArchiveFile.Services;
using MergeArchiveFile.Services.Interface;
using MergeArchiveFile.UtilityHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace MergeArchiveFile.CustomMiddleware
{
    public static class ServiceExtensions
    {
        public static void ConfigureServiceInjection(this IServiceCollection services)
        {
            services.AddScoped<IMergeFilesService, MergeFilesService>();
        }
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void ConfigureRateLimit(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        public static void ConfigureCustomExceptionMiddleware(this WebApplication app) => app.UseMiddleware<ErrorHandlerMiddleware>();

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Zip file merge, API",
                    Version = "v1",
                    Description = "A sampe of API to demonstrate Swagger",
                    Contact = new OpenApiContact
                    {
                        Name = "Driton Prushi",
                        Email = "prushidriton@gmail.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "It's free of use"
                    }
                });
            });
        }

    }
}
