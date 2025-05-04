using Api.Events;
using Api.Features.Accounts;
using Api.Features.Accounts.Send;
using Api.Tests.Integration.Tests.Abstract;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Tests
{
    [Collection(nameof(DatabaseTestCollection))]
    public class SendApiTests(IntegrationTestFactory factory) : BaseApiTests(factory)
    {
        private static string GetSendEndpoint(Guid fromAccountId) => $"api/v1/accounts/{fromAccountId}/send";

        Guid fromAccountId;
        Guid toAccountId;

        protected override async Task SeedDb()
        {
            await base.SeedDb();

            AccountCreated accountCreated = new(
                GetSeedUserId("customer01"),
                GetSeedUserId("teller"),
                "xxx-xxxxxx-x",
                1000
            );
            fromAccountId = session.Events.StartStream<BankAccount>(accountCreated).Id;

            AccountCreated accountCreated_2 = new(
                GetSeedUserId("customer02"),
                GetSeedUserId("teller"),
                "xxx-xxxxxx-x",
                0
            );
            toAccountId = session.Events.StartStream<BankAccount>(accountCreated_2).Id;

            await session.SaveChangesAsync();

        }

        [Fact]
        public async Task Send_Money_Should_Success()
        {
            await SeedDb();

            // Arrange
            decimal amount = 200;
            var body = new SendRequest()
            {
                ToAccountId = toAccountId,
                Amount = amount,
            };

            // Act
            await LoginBySeedUserAsync("customer01");
            var res = await client.PostAsJsonAsync(GetSendEndpoint(fromAccountId), body);
            res.EnsureSuccessStatusCode();

            // Assert
            var fromAccount = await session.Events.AggregateStreamAsync<BankAccount>(fromAccountId);
            var toAccount = await session.Events.AggregateStreamAsync<BankAccount>(toAccountId);

            Assert.NotNull(fromAccount);
            Assert.Equal(800,fromAccount.Balance);
            Assert.NotNull(toAccount);
            Assert.Equal(200,toAccount.Balance);
        }

    }
}
