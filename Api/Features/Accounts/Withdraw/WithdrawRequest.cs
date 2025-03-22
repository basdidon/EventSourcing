using FastEndpoints;

namespace Api.Features.Accounts.Withdraw
{
    public class WithdrawRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
