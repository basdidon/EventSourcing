using FastEndpoints;

namespace Api.Features.Accounts.Withdraw.Confirm
{
    public class WithdrawConfirmRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }

        public Guid RequestId { get; set; }

        public string Otp {  get; set; } = string.Empty;
    }
}
