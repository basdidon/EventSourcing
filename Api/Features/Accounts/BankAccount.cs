namespace Api.Features.Accounts
{
    public class BankAccount
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string AccountNumber { get; set; } = string.Empty;  // xxx-xxxxxx-x

        public decimal Balance { get; set; }
    }
}