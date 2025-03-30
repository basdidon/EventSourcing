using FastEndpoints;
using Marten;

namespace Api.Features.Accounts.GetAccountById
{
    public class GetAccountByIdEndpoint(IDocumentSession session): Endpoint<GetAccountByIdRequest,BankAccount>
    {
        public override void Configure()
        {
            Get("/accounts/{AccountId}");
        }
        
        public override async Task HandleAsync(GetAccountByIdRequest req, CancellationToken ct)
        {
            var account = await session.Events.AggregateStreamAsync<BankAccount>(req.AccountId,token: ct);

            if (account is null)
            {
                await SendNotFoundAsync(ct);
            }
            else if (account.OwnerId != req.UserId)
            {
                Console.WriteLine("[Forbidden]");
                Console.WriteLine($"ownerId:{account.OwnerId}");
                Console.WriteLine($"userId:{req.UserId}");
                await SendForbiddenAsync(ct);
            }
            else
            {
                await SendOkAsync(account, ct);
            }
        }
    }
}
