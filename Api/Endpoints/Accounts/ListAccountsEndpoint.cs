using FastEndpoints;

namespace Api.Endpoints.Accounts
{
    public class ListAccountsRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
    }

    public class ListAccountsEndpoint : Endpoint<ListAccountsRequest>
    {
        public override void Configure()
        {
            Get("/accounts");
        }

        public override Task HandleAsync(ListAccountsRequest req, CancellationToken ct)
        {
            return base.HandleAsync(req, ct);
        }
    }
}
