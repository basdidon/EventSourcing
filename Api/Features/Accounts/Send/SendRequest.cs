using FastEndpoints;

namespace Api.Features.Accounts.Send
{
    public class SendRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
    }
}
