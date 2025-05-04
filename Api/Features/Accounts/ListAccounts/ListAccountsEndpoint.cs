using Api.Const;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.ListAccounts
{
    public class ListAccountsEndpoint(IDocumentSession session) : Endpoint<ListAccountsRequest,IEnumerable<BankAccount>>
    {
        public override void Configure()
        {
            Get("/accounts");
            Roles(Role.Customer);
            // default authentication scheme return NotFound(404) when unauthorize(401) and forbidden(403)
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(ListAccountsRequest req, CancellationToken ct)
        {
            var accounts = await session.Query<BankAccount>().Where(x => x.OwnerId == req.UserId).ToListAsync(ct);

            if(accounts is null)
            {
                await SendNotFoundAsync(ct);
            }
            else
            {
                await SendAsync(accounts,cancellation: ct);
            }
            
        }
    }
}
