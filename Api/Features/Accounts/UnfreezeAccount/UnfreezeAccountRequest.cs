using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Accounts.UnfreezeAccount
{

    public class UnfreezeAccountRequest
    {
        [FromClaim("UserId")]
        public Guid AdminId { get; set; }

        [FromRoute]
        public Guid AccountId { get; set; }
    }
}
