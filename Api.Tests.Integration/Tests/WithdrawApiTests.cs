using Api.Events;
using Api.Features.Accounts;
using Api.Features.Accounts.Withdraw;
using Api.Features.Accounts.Withdraw.Confirm;
using Api.Tests.Integration.Tests.Abstract;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Tests
{
    [Collection(nameof(DatabaseTestCollection))]
    public class WithdrawApiTests(IntegrationTestFactory factory) : BaseApiTests(factory)
    {
        private static string GetWithdrawEndpoint(Guid accountId) => $"api/v1/accounts/{accountId}/withdraw";
        private static string GetConfirmEndpoint(Guid requestId) => $"/api/v1/withdraw/{requestId}/confirm";

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
        public async Task Withdraw_Request_Should_Success()
        {
            await SeedDb();

            var body = new WithdrawRequest()
            {
                Amount = 500,
            };

            // Act
            await LoginBySeedUserAsync("teller");
            var res = await client.PostAsJsonAsync(GetWithdrawEndpoint(accountId), body);
            res.EnsureSuccessStatusCode();

            var withdrawal = context.Withdrawals.FirstOrDefault();
            Assert.NotNull(withdrawal);
            var confirmBody = new WithdrawConfirmRequest()
            {
                Otp = withdrawal.Otp
            };
            var confirmRes = await client.PostAsJsonAsync(GetConfirmEndpoint(withdrawal.Id),confirmBody);
            confirmRes.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(accountId, withdrawal.AccountId);
            Assert.Equal(body.Amount, withdrawal.Amount);

            var account = await session.LoadAsync<BankAccount>(accountId);
            Assert.NotNull(account);
            Assert.Equal(500, account.Balance);
        }
    }
}
