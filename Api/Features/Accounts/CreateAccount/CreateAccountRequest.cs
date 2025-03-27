using FastEndpoints;

namespace Api.Features.Accounts.CreateAccount
{
    public class CreateAccountRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }

        public Guid CustomerId { get; set; }
        public decimal InitialBalance { get; set; } = 0;
    }
}
