using FastEndpoints;

namespace Api.Endpoints.Accounts
{
    public class DepositRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }

    public class DepositEndpoint:Endpoint<DepositRequest>
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
