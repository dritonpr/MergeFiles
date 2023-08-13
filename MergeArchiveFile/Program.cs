using AspNetCoreRateLimit;
using MergeArchiveFile.CustomMiddleware;
using MergeArchiveFile.Services.Interface;
using MergeArchiveFile.UtilityHelpers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var maxContentLength = builder.Configuration.GetValue<long>("FileUploadSettings:MaximumAllowedContentLength");
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Limits.MaxRequestBodySize = maxContentLength;
});


// Add services to the container.
builder.Services.ConfigureServiceInjection();
builder.Services.ConfigureCors();

//Rate limit
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.ConfigureRateLimit();

builder.Host.UseSerilog((context, configuration) => 
        configuration.ReadFrom.Configuration(context.Configuration));

// manage http request limits
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = builder.Configuration.GetValue<int>("FileUploadSettings:MaximumFileSize");
    options.MultipartBodyLengthLimit = maxContentLength;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();

builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUploadSettings"));
var app = builder.Build();

app.UseIpRateLimiting();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zip file merge, API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.ConfigureCustomExceptionMiddleware();

app.Run();
