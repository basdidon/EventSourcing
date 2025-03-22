using FastEndpoints;

namespace Api.Features.Accounts.FreezeAccount
{
    public class FreezeAccountEndpoint : Endpoint<FreezeAccountRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{AccountId}/freeze");
            Roles("Administrator");
        }

        public override Task HandleAsync(FreezeAccountRequest req, CancellationToken ct)
        {
            return base.HandleAsync(req, ct);
        }
    }
}
