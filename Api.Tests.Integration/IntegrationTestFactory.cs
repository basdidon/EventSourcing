using Api.Entities;
using Api.Events.User;
using Api.Events;
using Api.Features.Accounts.ListAccounts;
using Api.Persistance;
using Api.Projections;
using Api.Services;
using Marten;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace Api.Tests.Integration
{
    public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer identityDbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest") // Change this to same version as your production dataabse!
            .WithDatabase("identity")
            .WithUsername("postgres")
            .WithPassword("testpassword")
            .WithCleanUp(true)
            .Build();

        private readonly PostgreSqlContainer martenDbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("events")
            .WithUsername("postgres")
            .WithPassword("testpassword")
            .WithCleanUp(true)
            .Build();


        private Respawner _respawner = null!;
        private DbConnection _connection = null!;

        public RoleService RoleService { get; private set; } = null!;
        public UserService UserService { get; private set; } = null!;
        public IDocumentSession Session { get; private set; } = null!;
        public ApplicationDbContext Db { get; private set; } = null!;

        public async Task ResetDatabase()
        {
            await _respawner.ResetAsync(_connection);
        }

        public async Task InitializeAsync()
        {
            await identityDbContainer.StartAsync();
            await martenDbContainer.StartAsync();

            Db = Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            RoleService = Services.CreateScope().ServiceProvider.GetRequiredService<RoleService>();
            UserService = Services.CreateScope().ServiceProvider.GetRequiredService<UserService>();
            Session = Services.CreateScope().ServiceProvider.GetRequiredService<IDocumentSession>();

            _connection = Db.Database.GetDbConnection();
            await _connection.OpenAsync();

            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"]
            });
        }

        public new async Task DisposeAsync()
        {
            await _connection.CloseAsync();
            await identityDbContainer.DisposeAsync();
            await martenDbContainer.DisposeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<ApplicationDbContext>();
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(identityDbContainer.GetConnectionString());
                });
                services.EnsureDbCreated<ApplicationDbContext>();

                services.RemoveAll(typeof(IDocumentStore));
                services.AddMarten(options =>
                {
                    options.Connection(martenDbContainer.GetConnectionString()); // Use the test DB
                    options.UseSystemTextJsonForSerialization();

                    // Register events (if your tests need event sourcing)
                    options.Events.AddEventType<UserRegistered>();
                    options.Events.AddEventType<AccountCreated>();
                    options.Events.AddEventType<MoneyDeposited>();
                    options.Events.AddEventType<MoneyWithdrawn>();
                    options.Events.AddEventType<MoneyTransfered>();
                    options.Events.AddEventType<AccountClosed>();

                    // Only add projections if your tests depend on them
                    options.Projections.Add<BankAccountProjection>(ProjectionLifecycle.Inline);
                    options.Projections.Add<UserAccountsProjection>(ProjectionLifecycle.Inline);

                    // Register schema objects if your tests involve these entities
                    options.Schema.For<UserAccounts>().Identity(x => x.UserId);
                    options.Schema.For<BankAccount>().Identity(x => x.Id);
                })
                .UseLightweightSessions();
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        }

        public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var context = serviceProvider.GetRequiredService<T>();
            context.Database.EnsureCreated();
        }
    }
}
