using FastEndpoints;
using Marten;

namespace Api.Features.Accounts.ListAccounts
{

    public class ListAccountsEndpoint(IDocumentSession session) : Endpoint<ListAccountsRequest,UserAccounts>
    {
        public override void Configure()
        {
            Get("/accounts");
        }

        public override async Task HandleAsync(ListAccountsRequest req, CancellationToken ct)
        {
            var userAccounts = await session.Events.AggregateStreamAsync<UserAccounts>(req.UserId,token:ct);
            
            if(userAccounts is null)
            {
                await SendNotFoundAsync(ct);
            }
            else
            {
                await SendAsync(userAccounts,cancellation: ct);
            }
            
        }
    }
}
