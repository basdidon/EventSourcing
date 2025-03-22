using FastEndpoints;

namespace Api.Features.Accounts.ListAccounts
{
    public class ListAccountsRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
    }
}
