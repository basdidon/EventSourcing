using Api.Const;
using Api.Events;
using Api.Events.User;
using Api.Features.Accounts;
using Api.Persistance;
using Api.Services;
using Marten;

namespace Api.Extensions
{

    public static class SeedDataExtensions
    {
        public static async Task SeedData(this IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();
            var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

            // reset events and documents
            await store.Advanced.ResetAllData();

            // seed roles
            await roleService.CreateRolesAsync(Role.All);

            // SEED USERS
            // Admin
            var admin = await userService.CreateUser("admin","admin123",[Role.Admin]);

            // Teller
            var teller = await userService.CreateUser("teller", "teller123", [Role.Teller]);

            // Customers
            var cust01 = await userService.CreateUser("customer01", "customer01",[Role.Customer]);
            var cust02 = await userService.CreateUser("customer02", "customer02",[Role.Customer]);

            // SEED TRANSACTIONS
            // teller create account for customers
            var account1_created = new AccountCreated( cust01.Id, teller.Id, "xxx-xxxxxx-x", 1000);
            var account1_withdrawn = new MoneyWithdrawn(300,teller.Id);
            var account1_deposited = new MoneyDeposited(800, teller.Id);
            session.Events.StartStream<BankAccount>(account1_created,account1_withdrawn,account1_deposited);

            await session.SaveChangesAsync();
        }
    }
}
