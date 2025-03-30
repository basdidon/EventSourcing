using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.Send
{

    public class SendEndpoint(IDocumentSession session) : Endpoint<SendRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{SenderId}/send");
            Roles("Customer");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(SendRequest req, CancellationToken ct)
        {
            // ensure both accounts existing
            var fromStream = await session.Events.FetchForWriting<BankAccount>(req.FromAccount, ct);
            var fromAccount = fromStream.Aggregate;
            if (fromAccount is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (fromAccount.OwnerId != req.UserId)
            {
                await SendErrorsAsync(400, ct);
            }

            if (fromAccount.Balance < req.Amount)
            {
                await SendErrorsAsync(400, ct);
            }

            var toStream = await session.Events.FetchForWriting<BankAccount>(req.ToAccount, ct);
            var toAccount = toStream.Aggregate;
            if (toAccount is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // process
        }
    }
}
