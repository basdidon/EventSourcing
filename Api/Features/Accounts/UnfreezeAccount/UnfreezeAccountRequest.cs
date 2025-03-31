using FastEndpoints;

namespace Api.Features.Accounts.UnfreezeAccount
{
    public class UnfreezeAccountRequest
    {
        [FromClaim("UserId")]
        public Guid AdminId { get; set; }

        public Guid AccountId { get; set; }
    }
}
