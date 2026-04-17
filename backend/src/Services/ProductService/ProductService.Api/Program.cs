using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using ProductService.Api.Endpoints;
using ProductService.Api.Extensions;
using ProductService.Application;
using ProductService.Domain;
using ProductService.Extensions;
using ProductService.Infrastructure;
using ProductService.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration[EnvironmentVariable.CONNECTIONSTRING] ?? string.Empty;

builder.Services.AddDbContext<ProductDbContext>(opts =>
    opts.UseNpgsql(connectionString)
        .UseLowerCaseNamingConvention()
           .ConfigureWarnings(warnings =>
               warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

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
            OnTokenValidated = (context) => Task.CompletedTask
        };
    });

builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddAuthorization();
builder.Services.AddCors();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.UseCors(config =>
{
    var allowedOrigins = builder.Configuration[EnvironmentVariable.API_ALLOWED_ORIGINS];
    if (!string.IsNullOrEmpty(allowedOrigins))
    {
        var origins = allowedOrigins.Split(',');
        config.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    }
});

app.UseHttpsRedirection();

app.RegisterEndpoints();

app.UseAuthentication();

app.UseAuthorization();

await app.ApplyMigrationCheks();

await app.RunAsync();


public partial class Program { }
