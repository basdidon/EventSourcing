using Api.Const;
using Api.Events;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.Deposit
{
    public class DepositEndpoint(IDocumentSession session) : Endpoint<DepositRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{AccountId}/deposit");
            Roles(Role.Teller, Role.Admin);
            // default authentication scheme return NotFound(404) when unauthorize(401) and forbidden(403)
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(DepositRequest req, CancellationToken ct)
        {
            var stream = await session.Events.FetchForWriting<BankAccount>(req.AccountId, ct);
            var account = stream.Aggregate;
            if (account is null) // account
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (account.IsFrozen)
            {
                AddError("account is frozen.");
                await SendErrorsAsync(409, ct);
                return;
            }

            stream.AppendOne(new MoneyDeposited( req.Amount, req.UserId));
            await session.SaveChangesAsync(ct);

            await SendOkAsync(ct);
        }
    }
}
