using FastEndpoints;

namespace Api.Features.Accounts.Send
{
    public class SendRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        
        public Guid FromAccount { get; set; }
        public Guid ToAccount { get; set; }
        public decimal Amount { get; set; }
    }
}
