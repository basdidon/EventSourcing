using Api.Events;
using Api.Features.Accounts;
using Api.Features.Accounts.Deposit;
using Api.Tests.Integration.Tests.Abstract;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Tests
{
    [Collection(nameof(DatabaseTestCollection))]
    public class DepositApiTests(IntegrationTestFactory factory) : BaseApiTests(factory)
    {
        private static string GetDepositEndpoint(Guid accountId) => $"api/v1/accounts/{accountId}/deposit";

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
        public async Task Deposit_Should_Success()
        {
            await SeedDb();

            // Arrange
            var body = new DepositRequest()
            {
                Amount = 500
            };

            // Act
            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(GetDepositEndpoint(accountId),body);
            res.EnsureSuccessStatusCode();

            // Assert
            var account = await session.Events.AggregateStreamAsync<BankAccount>(accountId);
            Assert.NotNull(account);
            Assert.Equal(1500, account.Balance);
        }


    }
}
