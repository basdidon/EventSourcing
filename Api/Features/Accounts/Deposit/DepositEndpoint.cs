using FastEndpoints;

namespace Api.Features.Accounts.Deposit
{

    public class DepositEndpoint : Endpoint<DepositRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{AccountId}/deposit");
        }

        public override Task HandleAsync(DepositRequest req, CancellationToken ct)
        {
            return base.HandleAsync(req, ct);
        }
    }
}
