using FastEndpoints;

namespace Api.Features.Accounts.FreezeAccount
{
    public class FreezeAccountRequest
    {
        [FromClaim("UserId")]
        public Guid AdminId { get; set; }

        [RouteParam]
        public Guid AccountId { get; set; }
    }
}
