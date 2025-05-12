using Api.Const;
using FastEndpoints;
using Marten;
using Marten.Pagination;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.ListAccounts
{
    public class ListAccountsEndpoint(IDocumentSession session) : Endpoint<ListAccountsRequest, IEnumerable<BankAccount>>
    {
        public override void Configure()
        {
            Get("/accounts");
            Roles(Role.All);
            // default authentication scheme return NotFound(404) when unauthorize(401) and forbidden(403)
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(ListAccountsRequest req, CancellationToken ct)
        {
            bool IsProvidedOwner = req.OwnerId is not null;
            bool IsTellerOrAdmin = User.IsInRole(Role.Teller) || User.IsInRole(Role.Admin);

            if (!IsTellerOrAdmin && !IsProvidedOwner)
            {
                // regular user list thier owned bank accounts
                Response = await session.Query<BankAccount>()
                    .Where(x => x.OwnerId == req.UserId)
                    .ToPagedListAsync(req.Page, req.PageSize, ct);                
            }
            else if(!IsTellerOrAdmin && IsProvidedOwner)
            {
                // regular user cannot access bank account that owned by other
                await SendForbiddenAsync(ct);
                return;
            }
            else if(IsTellerOrAdmin & IsProvidedOwner)
            {
                // Teller Or Admin get specific Account
                Response = await session.Query<BankAccount>()
                    .Where(x => x.OwnerId == req.OwnerId)
                    .ToPagedListAsync(req.Page, req.PageSize, ct);                
            }
            else
            {
                // Teller or Admin retrieve all bank account with pagination supported
                Response = await session.Query<BankAccount>()
                    .ToPagedListAsync(req.Page, req.PageSize, ct);
            }
        }
    }
}
