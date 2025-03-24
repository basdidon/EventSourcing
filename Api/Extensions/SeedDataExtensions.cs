using Api.Entities;
using Api.Events;
using Api.Features.Users;
using Api.Persistance;
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
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();
            var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

            // Ensure role exists before adding to user
            var roleExists = await roleManager.RoleExistsAsync("admin");
            if (!roleExists)
            {
                // Create the "admin" role if it doesn't exist
                var role = new ApplicationRole { Name = "admin" };
                await roleManager.CreateAsync(role);
            }

            ApplicationUser adminUser = new()
            {
                UserName = "admin",
            };
            await userManager.CreateAsync(adminUser, "admin123");
            await context.SaveChangesAsync();
            Console.WriteLine($"adminID : {adminUser.Id}");
            
            await userManager.AddToRoleAsync(adminUser, "admin");

            await store.Advanced.ResetAllData();

            var userId = Guid.Parse("B64C2B02-0D4B-420A-825C-CC42C5CA80CA");

            var account1_id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
            var account1_created = new AccountCreated(userId,account1_id, "xxx-xxxxxx-x", 1000);
            var account1_deposit = new MoneyDeposited(account1_id, 500);

            var streamId1 = session.Events.StartStream<BankAccount>(account1_id,account1_created, account1_deposit).Id;

            var account2_id = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
            var account2_created = new AccountCreated(userId, account2_id, "xxx-xxxxxx-x", 1000);
            var account2_withdrawn = new MoneyWithdrawn(account2_id, 300);

            var streamId2 = session.Events.StartStream<BankAccount>(account2_id,account2_created, account2_withdrawn).Id;

            var account3_id = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC");
            var account3_created = new AccountCreated(userId,account3_id, "xxx-xxxxxx-x",500);
            var account3_withdrawn = new MoneyWithdrawn(account3_id, 500);
            var account3_closed = new AccountClosed(account3_id);

            session.Events.StartStream<BankAccount>(account3_id,account3_created,account3_withdrawn,account3_closed);

            await session.SaveChangesAsync();

            // sent
            var account1_sent = new MoneyTransfered(account1_id, account2_id, 500);
            session.Events.Append(streamId1, account1_sent);
            session.Events.Append(streamId2, account1_sent);
            await session.SaveChangesAsync();

        }
    }
}
