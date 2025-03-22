using Api.Events;
using Api.Features.Accounts.GetAccountById;
using FastEndpoints;
using Marten;

namespace Api.Features.Accounts.CreateAccount
{
    public class CreateAccountEndpoint(IDocumentSession session) : Endpoint<CreateAccountRequest, CreateAccountResponse>
    {
        public override void Configure()
        {
            Post("/accounts");
        }

        public override async Task HandleAsync(CreateAccountRequest req, CancellationToken ct)
        {
            var accountId = Guid.NewGuid();
            var accountNumber = "xxx-xxxxxx-x";
            session.Events.StartStream(accountId, new AccountCreated(accountId, req.UserId, accountNumber, req.InitialBalance));
            await session.SaveChangesAsync(ct);

            await SendCreatedAtAsync<GetAccountByIdEndpoint>(
                new
                {
                    AccountId = accountId
                },
                new()
                {
                    AccountId = accountId,
                    AccountNumber = accountNumber
                },
                cancellation: ct);
        }
    }
}
