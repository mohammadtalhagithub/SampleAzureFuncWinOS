using BeeSys.Utilities;
using BeeSys.Utilities.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

var config = builder.Configuration;


builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddScoped<CDownloadArcFile>();

// => Add Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = config["Jwt:Authority"];   // from appsettings
        options.Audience = config["Jwt:Audience"];
        options.RequireHttpsMetadata = true;
    });

// => Add Authorization
builder.Services.AddAuthorization();

// => Add Middleware
builder.UseMiddleware<JwtAuthMiddleware>();



builder.Build().Run();
