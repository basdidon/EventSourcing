using Api.Const;
using Api.Events;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.FreezeAccount
{
    public class FreezeAccountEndpoint(IDocumentSession session) : Endpoint<FreezeAccountRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{AccountId}/freeze");
            Roles(Role.Admin);
            // default authenstication scheme return NotFound(404) when unauthorize(401) and forbidden(403)
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(FreezeAccountRequest req, CancellationToken ct)
        {
            var stream = await session.Events.FetchForWriting<BankAccount>(req.AccountId,ct);
            var account = stream.Aggregate;
            if(account is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (account.IsFrozen)
            {
                AddError("account already frozen.");
                await SendErrorsAsync(409,ct);
                return;
            }

            var e = new AccountFrozen(req.AccountId, account.OwnerId, req.AdminId);
            stream.AppendOne(e);
            await session.SaveChangesAsync(ct);
        }
    }
}
