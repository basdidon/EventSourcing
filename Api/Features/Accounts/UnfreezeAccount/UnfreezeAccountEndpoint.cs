using Api.Const;
using Api.Events;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.UnfreezeAccount
{
    public class UnfreezeAccountEndpoint(IDocumentSession session) : Endpoint<UnfreezeAccountRequest>
    {
        public override void Configure()
        {
            Post("accounts/{AccountId}/unfreeze");
            Roles(Role.Admin);
            // default authentication scheme return NotFound(404) when unauthorize(401) and forbidden(403)
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(UnfreezeAccountRequest req, CancellationToken ct)
        {
            var stream = await session.Events.FetchForWriting<BankAccount>(req.AccountId, ct);
            var account = stream.Aggregate;
            if (account is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (!account.IsFrozen)
            {
                AddError("account is not frozen.");
                await SendErrorsAsync(409, ct);
                return;
            }

            var e = new AccountUnfrozen(req.AccountId, account.OwnerId, req.AdminId);
            stream.AppendOne(e);
            await session.SaveChangesAsync(ct);
        }
    }
}
