using FastEndpoints;

namespace Api.Features.Accounts.Send
{
    public class SendRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }

}
