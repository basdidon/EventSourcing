namespace Api.Features.Accounts.CreateAccount
{
    public class CreateAccountResponse
    {
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
    }
}
