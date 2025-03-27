using Api.Entities;
using Api.Events;
using FastEndpoints;
using Marten;

namespace Api.Features.Accounts.Deposit
{

    public class DepositEndpoint(IDocumentSession session) : Endpoint<DepositRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{AccountId}/deposit");
            Roles("Teller","Admin");
        }

        public override async Task HandleAsync(DepositRequest req, CancellationToken ct)
        {
            var stream = await session.Events.FetchForWriting<BankAccount>(req.AccountId, ct);
            var ownerId = stream.Aggregate.OwnerId;

            stream.AppendOne(new MoneyDeposited(req.AccountId, req.UserId, ownerId, req.Amount));
            await session.SaveChangesAsync(ct);
        }
    }
}
