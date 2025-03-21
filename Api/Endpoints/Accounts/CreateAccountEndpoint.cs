using FastEndpoints;

namespace Api.Endpoints.Accounts
{
    public class CreateAccountRequest
    {
        public Guid UserId { get; set; }
        public decimal InitialBalance { get; set; } = 0;
    }

    public class CreateAccountResponse
    {
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
    }

    public class CreateAccountEndpoint : Endpoint<CreateAccountRequest,CreateAccountResponse>
    {
        public override void Configure()
        {
            Post("/accounts");
        }

        public override Task<CreateAccountResponse> ExecuteAsync(CreateAccountRequest req, CancellationToken ct)
        {
            return base.ExecuteAsync(req, ct);
        }
    }
}
