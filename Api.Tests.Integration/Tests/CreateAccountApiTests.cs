using Api.Features.Accounts;
using Api.Features.Accounts.CreateAccount;
using Api.Tests.Integration.Tests.Abstract;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Tests.Integration.Tests
{
    [Collection(nameof(DatabaseTestCollection))]
    public class CreateAccountApiTests(IntegrationTestFactory factory) : BaseApiTests(factory)
    {
        const string requestEndpoint = "/api/v1/accounts";

        #region Initial Balance
        [Fact]
        public async Task Create_Account_Initial_Balance_Zero_Should_Success()
        {
            await SeedDb();

            // Arrange
            var customer01_id = GetSeedUserId("customer01");
            var body = new CreateAccountRequest
            {
                CustomerId = customer01_id,
                InitialBalance = 0,
            };

            // Act
            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(requestEndpoint, body);
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

            // Document
            var account = await session.LoadAsync<BankAccount>(accountId);
            Assert.NotNull(account);
            Assert.Equal(0, account.Balance);
            Assert.Equal(customer01_id, account.OwnerId);
        }

        [Fact]
        public async Task Create_Account_Without_Initial_Balance_Should_Success()
        {
            await SeedDb();

            // Arrange
            var customer01_id = GetSeedUserId("customer01");
            var body = new CreateAccountRequest
            {
                CustomerId = customer01_id,
            };

            // Act
            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(requestEndpoint, body);
            res.EnsureSuccessStatusCode();
            var jsonString = await res.Content.ReadAsStringAsync();

            // Assert

            // response
            var json = JsonSerializer.Deserialize<JsonElement>(jsonString);
            Assert.True(json.TryGetProperty("accountId", out var accountIdElement), "accountId property not found in response");
            bool isValidGuid = Guid.TryParse(accountIdElement.GetString(), out Guid accountId);
            Assert.True(isValidGuid, "accountId is not a valid GUID");
            // Persistance
            var account = await session.LoadAsync<BankAccount>(accountId);
            Assert.NotNull(account);
            Assert.Equal(0, account.Balance);  // default value
            Assert.Equal(customer01_id, account.OwnerId);
        }

        [Fact]
        public async Task Create_Account_With_Possitive_Initial_Balance_Should_Success()
        {
            await SeedDb();

            // Arrange
            var customer01_id = GetSeedUserId("customer01");
            var initialBalance = 25000;
            var body = new CreateAccountRequest
            {
                CustomerId = customer01_id,
                InitialBalance = initialBalance
            };

            // Act
            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(requestEndpoint, body);
            res.EnsureSuccessStatusCode();
            var jsonString = await res.Content.ReadAsStringAsync();

            // Assert

            // response
            var json = JsonSerializer.Deserialize<JsonElement>(jsonString);
            Assert.True(json.TryGetProperty("accountId", out var accountIdElement), "accountId property not found in response");
            bool isValidGuid = Guid.TryParse(accountIdElement.GetString(), out Guid accountId);
            Assert.True(isValidGuid, "accountId is not a valid GUID");
            // Persistance
            var account = await session.LoadAsync<BankAccount>(accountId);
            Assert.NotNull(account);
            Assert.Equal(initialBalance, account.Balance);
            Assert.Equal(customer01_id, account.OwnerId);
        }

        [Fact]
        public async Task Create_Account_With_Negative_Initial_Balance_Should_Failed()
        {
            await SeedDb();

            // Arrange
            var customer01_id = GetSeedUserId("customer01");
            var initialBalance = -2000;
            var body = new CreateAccountRequest
            {
                CustomerId = customer01_id,
                InitialBalance = initialBalance
            };

            // Act
            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(requestEndpoint, body);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }
        #endregion

        [Fact]
        public async Task Create_Account_With_Non_Exists_Customer_Should_Failed()
        {
            await SeedDb();

            // Arrange
            var body = new CreateAccountRequest
            {
                CustomerId = Guid.NewGuid(),
                InitialBalance = 1000,
            };

            // Act
            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(requestEndpoint, body);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task Create_Account_For_Teller_Should_Failed()
        {
            await SeedDb();

            // Arrange
            var body = new CreateAccountRequest
            {
                CustomerId = GetSeedUserId("teller"), // use teller id
                InitialBalance = 1000,
            };

            // Act
            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(requestEndpoint, body);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }

        [Fact]
        public async Task Create_Account_For_Admin_Should_Failed()
        {
            await SeedDb();

            // Arrange
            var body = new CreateAccountRequest
            {
                CustomerId = GetSeedUserId("admin"), // use admin id
                InitialBalance = 1000,
            };

            // Act
            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(requestEndpoint, body);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }

        [Fact]
        public async Task Create_Account_By_Admin_Should_Success()
        {
            await SeedDb();

            // Arrange
            var body = new CreateAccountRequest
            {
                CustomerId = GetSeedUserId("customer01"),
                InitialBalance = 1000,
            };

            // Act
            await LoginBySeedUserAsync("admin");
            var res = await client.PostAsJsonAsync(requestEndpoint, body);

            // Assert
            res.EnsureSuccessStatusCode();
        }
    }
}
