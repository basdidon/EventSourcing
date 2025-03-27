using Api.Entities;
using Api.Enums;
using Api.Events;
using Api.Events.User;
using Api.Features.Accounts.ListAccounts;
using Api.Features.Users;
using Api.Persistance;
using Api.Services;
using Marten;
using Microsoft.AspNetCore.Identity;

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
            await roleService.CreateRolesAsync([..Enum.GetNames<Roles>()]);

            // SEED USERS
            // Admin
            var admin = await userService.CreateUser("admin","admin123",Roles.Admin);

            // Teller
            var teller = await userService.CreateUser("teller", "teller123", Roles.Teller);

            // Customers
            var cust01 = await userService.CreateUser("customer01", "customer01",Roles.Customer);
            var cust02 = await userService.CreateUser("customer02", "customer02",Roles.Customer);

            // SEED TRANSACTIONS
            // teller create account for customers
            var account1_id = Guid.NewGuid();
            var account1_created = new AccountCreated(account1_id, teller.Id, cust01.Id, "xxx-xxxxxx-x", 1000);
            var account2_withdrawn = new MoneyWithdrawn(account1_id, teller.Id, cust01.Id, 300);
            var account1_deposited = new MoneyDeposited(account1_id, teller.Id, cust01.Id, 800);
            session.Events.StartStream<BankAccount>(account1_id,account1_created,account2_withdrawn,account1_deposited);

            await session.SaveChangesAsync();
        }
    }
}
