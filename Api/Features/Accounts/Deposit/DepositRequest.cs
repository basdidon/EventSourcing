using FastEndpoints;

namespace Api.Features.Accounts.Deposit
{
    public class DepositRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
