using FastEndpoints;

namespace Api.Features.Accounts.ListAccounts
{

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
