using Api.Persistance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Test
{
    internal class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithName("postgres:latest")
            .WithDatabase("")
            .WithUsername("")
            .WithPassword("")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if(descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(opts =>
                {
                    opts
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention();
                });
            });
        }
    }
}
