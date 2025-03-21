using FastEndpoints;

namespace Api.Endpoints.Accounts
{
    public class SendRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
    }

    public class SendEndpoint : Endpoint<SendRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{SenderId}/send");
        }

        public override Task HandleAsync(SendRequest req, CancellationToken ct)
        {
            return base.HandleAsync(req, ct);
        }
    }
}
