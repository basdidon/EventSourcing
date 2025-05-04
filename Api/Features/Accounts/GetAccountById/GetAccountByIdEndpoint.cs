using Api.Const;
using FastEndpoints;
using Marten;

namespace Api.Features.Accounts.GetAccountById
{
    public class GetAccountByIdEndpoint(IDocumentSession session): Endpoint<GetAccountByIdRequest,BankAccount>
    {
        public override void Configure()
        {
            Get("/accounts/{AccountId}");
            Roles(Role.All);
        }

        public override async Task HandleAsync(GetAccountByIdRequest req, CancellationToken ct)
        {
            var account = await session.Events.AggregateStreamAsync<BankAccount>(req.AccountId, token: ct);

            if (account is null)
            {
                await SendNotFoundAsync(ct);
            }
            else if (!User.IsInRole("Admin") && !User.IsInRole("Teller") && account.OwnerId != req.UserId)
            {
                await SendForbiddenAsync(ct);
            }
            else
            {
                await SendOkAsync(account, ct);
            }
        }
    }
}
