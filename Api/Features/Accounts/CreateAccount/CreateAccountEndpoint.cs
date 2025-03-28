using Api.Events;
using Api.Features.Accounts.GetAccountById;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.CreateAccount
{
    public class CreateAccountEndpoint(IDocumentSession session) : Endpoint<CreateAccountRequest, CreateAccountResponse>
    {
        public override void Configure()
        {
            Post("/accounts");
            Roles("Teller", "Admin");
            // default authentication scheme return NotFound(404) when unauthorize(401) and forbidden(403)
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(CreateAccountRequest req, CancellationToken ct)
        {
            var accountId = Guid.NewGuid();
            var accountNumber = "xxx-xxxxxx-x";

            // TODO : ensure CustomerId should exists

            session.Events.StartStream(accountId, new AccountCreated(accountId, req.UserId, req.CustomerId, accountNumber, req.InitialBalance));
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
