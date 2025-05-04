using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Accounts.Send
{
    public class SendRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }

        [FromRoute]
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }

}
