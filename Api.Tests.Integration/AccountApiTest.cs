using Api.Entities;
using Api.Enums;
using Api.Events;
using Api.Features.Accounts.CreateAccount;
using Api.Persistance;
using Api.Services;
using Marten;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Tests.Integration
{
    [Collection(nameof(DatabaseTestCollection))]
    public class AccountApiTests(IntegrationTestFactory factory)
    {
        protected readonly HttpClient client = factory.CreateClient();
        protected readonly RoleService roleService = factory.RoleService;
        protected readonly UserService userService = factory.UserService;
        protected readonly IDocumentSession session = factory.Session;
        protected readonly ApplicationDbContext context = factory.Db;
        protected readonly Func<Task> _resetDatabase = factory.ResetDatabase;

        public UserLoginDictionary SeedUsers = [];

        protected async Task SeedUser(string username, string password, Roles role)
        {
            var user = await userService.CreateUser(username, password, role);
            SeedUsers.Add(username, new()
            {
                Id = user.Id,
                Username = username,
                Password = password,
            });
        }

        protected async Task SeedDb()
        {
            await _resetDatabase();

            await roleService.CreateRolesAsync([.. Enum.GetNames<Roles>()]);

            // SEED USERS
            await SeedUser("admin","admin123",Roles.Admin);
            await SeedUser("teller", "teller123", Roles.Teller);
            await SeedUser("customer01", "customer01", Roles.Customer);
            await SeedUser("customer02", "customer02", Roles.Customer);

            // SEED TRANSACTIONS
            // teller create account for customers
            var account1_id = Guid.NewGuid();
            var account1_created = new AccountCreated(account1_id, SeedUsers["teller"].Id, SeedUsers["customer01"].Id, "xxx-xxxxxx-x", 1000);
            var account2_withdrawn = new MoneyWithdrawn(account1_id, SeedUsers["teller"].Id, SeedUsers["customer01"].Id, 300);
            var account1_deposited = new MoneyDeposited(account1_id, SeedUsers["teller"].Id, SeedUsers["customer01"].Id, 800);
            session.Events.StartStream<BankAccount>(account1_id, account1_created, account2_withdrawn, account1_deposited);

            await session.SaveChangesAsync();
        }



        [Fact]
        public async Task Create_Account_Initial_Balance_Zero_Should_Success()
        {
            await SeedDb();

            // Arrange
            var request = "/api/v1/accounts";
            var body = new CreateAccountRequest
            {
                CustomerId = SeedUsers["customer01"].Id,
                InitialBalance = 0,
            };

            // Act
            await AuthHelper.LoginAsTeller(client);
            var res = await client.PostAsJsonAsync(request, body);
            res.EnsureSuccessStatusCode();
            var jsonString = await res.Content.ReadAsStringAsync();

            // Assert
            var json = JsonSerializer.Deserialize<JsonElement>(jsonString);

            // Check if "accountId" property exists
            Assert.True(json.TryGetProperty("accountId", out var accountIdElement), "accountId property not found in response");

            // Try parsing the value as a GUID
            bool isValidGuid = Guid.TryParse(accountIdElement.GetString(), out Guid accountId);

            // Assert that accountId is a valid GUID
            Assert.True(isValidGuid, "accountId is not a valid GUID");
        }

        [Fact]
        public async Task Create_Account_With_Non_Exists_Customer_Should_Failed()
        {
            await SeedDb();

            // Arrange
            var request = "/api/v1/accounts";
            var body = new CreateAccountRequest
            {
                CustomerId = Guid.NewGuid(),
                InitialBalance = 1000,
            };

            // Act
            await AuthHelper.LoginAsTeller(client);
            var res = await client.PostAsJsonAsync(request, body);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task Create_Account_For_Teller_Should_Failed()
        {
            await SeedDb();

            // Arrange
            var request = "/api/v1/accounts";
            var body = new CreateAccountRequest
            {
                CustomerId = SeedUsers["teller"].Id, // use teller id
                InitialBalance = 1000,
            };

            // Act
            await AuthHelper.LoginAsTeller(client);
            var res = await client.PostAsJsonAsync(request, body);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }
    }
}
