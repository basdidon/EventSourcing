using Api.Const;
using FastEndpoints;
using Marten;
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
            IReadOnlyList<BankAccount>? accounts;
            if (req.OwnerId is not null)
            {
                if (!User.IsInRole(Role.Admin) && !User.IsInRole(Role.Teller))
                {
                    await SendForbiddenAsync(ct);
                    return;
                }

                accounts = await session.Query<BankAccount>()
                    .Where(x => x.OwnerId == req.OwnerId)
                    .ToListAsync(ct);
            }
            else if (User.IsInRole(Role.Teller) || User.IsInRole(Role.Admin))
            {
                accounts = await session.Query<BankAccount>()
                    .ToListAsync(ct);
            }
            else
            {
                accounts = await session.Query<BankAccount>()
                    .Where(x => x.OwnerId == req.UserId)
                    .ToListAsync(ct);
            }

            if (accounts is null)
            {
                await SendNotFoundAsync(ct);
            }
            else
            {
                await SendAsync(accounts, cancellation: ct);
            }
        }
    }
}
