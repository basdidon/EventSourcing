using Api.Events;
using Api.Features.Accounts;
using Api.Features.Accounts.Deposit;
using Api.Features.Accounts.FreezeAccount;
using Api.Tests.Integration.Tests.Abstract;
using System.Net;
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

            AccountCreated accountCreated = new(
                GetSeedUserId("customer01"),
                GetSeedUserId("teller"),
                "xxx-xxxxxx-x",
                1000
            );

            accountId = session.Events.StartStream<BankAccount>(accountCreated).Id;
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
            var res = await client.PostAsJsonAsync(GetDepositEndpoint(accountId), body);
            res.EnsureSuccessStatusCode();

            // Assert
            var account = await session.Events.AggregateStreamAsync<BankAccount>(accountId);
            Assert.NotNull(account);
            Assert.Equal(1500, account.Balance);
        }


        [Fact]
        public async Task Deposit_With_Nagative_Amount_Should_Failed()
        {
            await SeedDb();

            // Arrange
            var body = new DepositRequest()
            {
                Amount = -500
            };

            // Act
            await LoginBySeedUserAsync("teller");
            await client.PostAsJsonAsync(GetDepositEndpoint(accountId), body);

            // Assert
            var account = await session.Events.AggregateStreamAsync<BankAccount>(accountId);
            Assert.NotNull(account);
            Assert.Equal(1000, account.Balance);
        }

        [Fact]
        public async Task Deposit_To_Frozen_Account_Should_Failed()
        {
            await SeedDb();

            // Arrange
            var body = new DepositRequest()
            {
                Amount = 500
            };

            await LoginBySeedUserAsync("admin");
            var freezeRes = await client.PostAsJsonAsync(
                $"/api/v1/accounts/{accountId}/freeze",
                new FreezeAccountRequest()
                {
                    AccountId = accountId
                });
            freezeRes.EnsureSuccessStatusCode();

            // Act

            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(GetDepositEndpoint(accountId), body);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);
        }

    }
}
