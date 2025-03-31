using Api.Const;
using Api.Persistance;
using Api.Services;
using Api.Tests.Integration.Helper;
using Marten;

namespace Api.Tests.Integration.Tests.Abstract
{
    public abstract class BaseApiTests(IntegrationTestFactory factory)
    {
        protected readonly HttpClient client = factory.CreateClient();
        protected readonly RoleService roleService = factory.RoleService;
        protected readonly UserService userService = factory.UserService;
        protected readonly IDocumentSession session = factory.Session;
        protected readonly ApplicationDbContext context = factory.Db;
        protected readonly Func<Task> _resetDatabase = factory.ResetDatabase;

        private readonly UserLoginDictionary SeedUsers = [];

        protected async Task SeedUser(string username, string password, string[] roles)
        {
            var user = await userService.CreateUser(username, password, roles);
            SeedUsers.Add(username, new()
            {
                Id = user.Id,
                Username = username,
                Password = password,
            });
        }

        protected Guid GetSeedUserId(string username) => SeedUsers[username].Id;
        protected async Task LoginBySeedUserAsync(string username)
        {
            await AuthHelper.LoginAsync(client, username, SeedUsers[username].Password);
        }


        protected virtual async Task SeedDb()
        {
            await _resetDatabase();

            await roleService.CreateRolesAsync(Role.All);

            // SEED USERS
            await SeedUser("admin", "admin123", [Role.Admin]);
            await SeedUser("teller", "teller123", [Role.Teller]);
            await SeedUser("customer01", "customer01", [Role.Customer]);
            await SeedUser("customer02", "customer02", [Role.Customer]);
            /*
            // SEED TRANSACTIONS
            // teller create account for customers
            var account1_id = Guid.NewGuid();
            var account1_created = new AccountCreated(account1_id, SeedUsers["teller"].Id, SeedUsers["customer01"].Id, "xxx-xxxxxx-x", 1000);
            var account2_withdrawn = new MoneyWithdrawn(account1_id, SeedUsers["teller"].Id, SeedUsers["customer01"].Id, 300);
            var account1_deposited = new MoneyDeposited(account1_id, SeedUsers["teller"].Id, SeedUsers["customer01"].Id, 800);
            session.Events.StartStream<BankAccount>(account1_id, account1_created, account2_withdrawn, account1_deposited);

            await session.SaveChangesAsync();*/
        }
    }
}
