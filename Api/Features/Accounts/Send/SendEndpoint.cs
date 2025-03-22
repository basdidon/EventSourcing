using FastEndpoints;

namespace Api.Features.Accounts.Send
{

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
