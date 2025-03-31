using FastEndpoints;

namespace Api.Features.Accounts.GetAccountById
{
    public class GetAccountByIdRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }

        public Guid AccountId { get; set; }
    }
}
