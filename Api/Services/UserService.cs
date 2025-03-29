using Api.Enums;
using Api.Events.User;
using Api.Features.Users;
using Api.Persistance;
using Marten;
using Microsoft.AspNetCore.Identity;

namespace Api.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IDocumentSession session)
    {
        public async Task<ApplicationUser> CreateUser(string username, string password, Roles roles)
        {
            ApplicationUser user = new()
            {
                UserName = username
            };

            await userManager.CreateAsync(user, password);
            await context.SaveChangesAsync();  // required to save user before add role ?
            await userManager.AddToRoleAsync(user, roles.ToString());

            // Publish Event
            UserRegistered userRegistered = new(user.Id);
            session.Events.StartStream(userRegistered);
            await session.SaveChangesAsync();

            return user;
        }

        public async Task<ApplicationUser?> FindUser(Guid customerId)
        {
            return await userManager.FindByIdAsync(customerId.ToString());
        }
    }
}
