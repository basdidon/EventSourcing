using FastEndpoints;

namespace Api.Endpoints.Accounts
{
    public class GetAccountByIdRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
    }

    public class GetAccountByIdEndpoint : Endpoint<GetAccountByIdRequest>
    {
        public override void Configure()
        {
            Get("/accounts/{AccountId}");
        }

        public override Task HandleAsync(GetAccountByIdRequest req, CancellationToken ct)
        {
            return base.HandleAsync(req, ct);
        }
    }
}
