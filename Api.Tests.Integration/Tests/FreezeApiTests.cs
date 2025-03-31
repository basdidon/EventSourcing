using Api.Events;
using Api.Features.Accounts;
using Api.Features.Accounts.FreezeAccount;
using Api.Features.Accounts.UnfreezeAccount;
using Api.Tests.Integration.Tests.Abstract;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Tests
{
    [Collection(nameof(DatabaseTestCollection))]
    public class FreezeApiTests(IntegrationTestFactory factory) : BaseApiTests(factory)
    {
        private static string GetFreezeEndpoint(Guid accountId) => $"api/v1/accounts/{accountId}/freeze";
        private static string GetUnfreezeEndpoint(Guid accountId) => $"api/v1/accounts/{accountId}/unfreeze";

        Guid accountId;

        protected override async Task SeedDb()
        {
            await base.SeedDb();

            accountId = Guid.NewGuid();
            AccountCreated accountCreated = new(
                accountId,
                GetSeedUserId("teller"),
                GetSeedUserId("customer01"),
                "xxx-xxxxxx-x",
                1000
            );

            session.Events.StartStream<BankAccount>(accountId, accountCreated);
            await session.SaveChangesAsync();
        }

        [Fact]
        public async Task Freeze_Account_Should_Success()
        {
            await SeedDb();

            // Arrange
            var body = new FreezeAccountRequest()
            {
                AccountId = accountId,
            };

            // Act
            await LoginBySeedUserAsync("admin");
            var res = await client.PostAsJsonAsync(GetFreezeEndpoint(accountId), body);
            res.EnsureSuccessStatusCode();

            // Assert
            var account = await session.Events.AggregateStreamAsync<BankAccount>(accountId);
            Assert.NotNull(account);
            Assert.True(account.IsFrozen);
        }

        [Fact]
        public async Task Unfreeze_Account_Should_Success()
        {
            await SeedDb();

            // Arrange
            var body = new UnfreezeAccountRequest()
            {
                AccountId = accountId,
            };

            await LoginBySeedUserAsync("admin");
            var freezeRes = await client.PostAsJsonAsync(GetFreezeEndpoint(accountId), new FreezeAccountRequest(){ AccountId = accountId });
            freezeRes.EnsureSuccessStatusCode();
            // Act
            var res = await client.PostAsJsonAsync(GetUnfreezeEndpoint(accountId), body);
            res.EnsureSuccessStatusCode();

            // Assert
            var account = await session.Events.AggregateStreamAsync<BankAccount>(accountId);
            Assert.NotNull(account);
            Assert.False(account.IsFrozen);
        }
    }
}
