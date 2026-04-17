using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure;
using Testcontainers.PostgreSql;

namespace ProductService.IntegrationTests.Config;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresSqlTestContainer = new PostgreSqlBuilder("postgres:17")
        .WithDatabase($"test-{Guid.NewGuid}")
        .WithUsername("postgres")
        .WithPassword("password")
        .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilCommandIsCompleted("pg_isready"))
        .Build();

    public ApiWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ConfigureServices);
        builder.ConfigureLogging((context, loggingBuilder) => loggingBuilder.ClearProviders());
    }

    private void ConfigureServices(IServiceCollection services)
    {
        RemoveServices(services, typeof(DbContextOptions<ProductDbContext>),
            typeof(AuthenticationHandler<AuthenticationSchemeOptions>));

        services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });

        services.AddDbContext<ProductDbContext>(opts =>
            opts.UseNpgsql(_postgresSqlTestContainer.GetConnectionString())
                .UseLowerCaseNamingConvention()
                   .ConfigureWarnings(warnings =>
                       warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));
    }

    private static void RemoveServices(IServiceCollection services, params Type[] types)
    {
        foreach (var type in types)
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == type);
            services.Remove(serviceDescriptor);
        }
    }

    public string ConnectionString => _postgresSqlTestContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgresSqlTestContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresSqlTestContainer.StopAsync();
    }
}
