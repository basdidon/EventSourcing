using FastEndpoints;
using Marten;

namespace Api.Features.Accounts.Deposit
{

    public class DepositEndpoint(IDocumentSession session) : Endpoint<DepositRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{AccountId}/deposit");
        }

        public override async Task HandleAsync(DepositRequest req, CancellationToken ct)
        {
            session.Events.Append(req.AccountId, req.Amount);
            await session.SaveChangesAsync(ct);
        }
    }
}
