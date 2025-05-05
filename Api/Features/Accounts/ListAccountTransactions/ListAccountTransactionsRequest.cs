using FastEndpoints;

namespace Api.Features.Accounts.ListAccountTransactions
{
    public class ListAccountTransactionsRequest
    {
        [FromClaim]
        public Guid UserId {  get; set; }

        [RouteParam]
        public Guid AccountId { get; set; }

    }

}
