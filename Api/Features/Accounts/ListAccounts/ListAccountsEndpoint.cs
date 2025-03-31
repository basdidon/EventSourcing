using Api.Const;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.ListAccounts
{
    public class ListAccountsEndpoint(IDocumentSession session) : Endpoint<ListAccountsRequest,UserAccounts>
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
            var userAccounts = await session.LoadAsync<UserAccounts>(req.UserId, ct);
            
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
