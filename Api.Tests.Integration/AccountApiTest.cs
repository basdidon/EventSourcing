using Api.Entities;
using Api.Enums;
using Api.Events;
using Api.Features.Accounts.CreateAccount;
using Api.Persistance;
using Api.Services;
using FastExpressionCompiler;
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

        private Guid adminId;
        private Guid tellerId;
        private Guid customer1_Id;
        private Guid customer2_Id;

        protected async Task SeedDb()
        {
            await _resetDatabase();

            await roleService.CreateRolesAsync([.. Enum.GetNames<Roles>()]);

            // SEED USERS
            // Admin
            var admin = await userService.CreateUser("admin", "admin123", Roles.Admin);
            adminId = admin.Id;

            // Teller
            var teller = await userService.CreateUser("teller", "teller123", Roles.Teller);
            tellerId = teller.Id;

            // Customers
            var cust01 = await userService.CreateUser("customer01", "customer01", Roles.Customer);
            customer1_Id = cust01.Id;
            var cust02 = await userService.CreateUser("customer02", "customer02", Roles.Customer);
            customer2_Id = cust02.Id;

            // SEED TRANSACTIONS
            // teller create account for customers
            var account1_id = Guid.NewGuid();
            var account1_created = new AccountCreated(account1_id, teller.Id, cust01.Id, "xxx-xxxxxx-x", 1000);
            var account2_withdrawn = new MoneyWithdrawn(account1_id, teller.Id, cust01.Id, 300);
            var account1_deposited = new MoneyDeposited(account1_id, teller.Id, cust01.Id, 800);
            session.Events.StartStream<BankAccount>(account1_id, account1_created, account2_withdrawn, account1_deposited);

            await session.SaveChangesAsync();
        }

        [Fact]
        public async Task HealthCheck()
        {
            string request = "/health";
            var response = await client.GetAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Create_Account_Initial_Balance_Zero_Should_Success()
        {
            await SeedDb();

            // Arrange
            var request = "/api/v1/accounts";
            var body = new CreateAccountRequest
            {
                CustomerId = customer1_Id,
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
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }
    }
}
