using ApiGateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
var tokenValidationParameters = new TokenValidationParameters
{
    ValidIssuer = builder.Configuration[EnvironmentVariable.AUTHORITY],
    ValidAudience = builder.Configuration[EnvironmentVariable.AUDIENCE],
    ValidateAudience = true,
    ValidateIssuer = true
};

if (builder.Environment.IsDevelopment())
{
    tokenValidationParameters.ValidateAudience = false;
    tokenValidationParameters.ValidateIssuer = false;
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = bool.Parse(builder.Configuration[EnvironmentVariable.REQUIREHTTPS]);
        o.Authority = builder.Configuration[EnvironmentVariable.AUTHORITY];
        o.TokenValidationParameters = tokenValidationParameters;

        o.Events = new JwtBearerEvents
        {
            OnTokenValidated = (context) => Task.CompletedTask,
            OnAuthenticationFailed = (context) =>
            {
                return Task.CompletedTask;
            }
        };
    });

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(config =>
{
    var allowedOrigins = builder.Configuration[EnvironmentVariable.API_ALLOWED_ORIGINS];
    if (!string.IsNullOrEmpty(allowedOrigins))
    {
        var origins = allowedOrigins.Split(',');
        config.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    }
});

app.UseAuthentication();

app.UseAuthorization();

await app.UseOcelot();

app.Run();
