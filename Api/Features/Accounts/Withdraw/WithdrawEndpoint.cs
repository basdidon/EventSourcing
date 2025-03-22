using FastEndpoints;

namespace Api.Features.Accounts.Withdraw
{
    public class WithdrawEndpoint : Endpoint<WithdrawRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{AccountId}/withdraw");
        }

        public override Task HandleAsync(WithdrawRequest req, CancellationToken ct)
        {
            return base.HandleAsync(req, ct);
        }
    }
}