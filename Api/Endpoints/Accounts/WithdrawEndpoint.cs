using FastEndpoints;

namespace Api.Endpoints.Accounts
{
    public class WithdrawRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }

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
