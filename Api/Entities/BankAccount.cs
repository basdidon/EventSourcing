namespace Api.Entities
{
    public class BankAccount
    {
        public Guid Id { get; set; }

        public string AccountNumber { get; set; } = string.Empty;  // xxx-xxxxxx-x

        public decimal Balance { get; set; }
    }
}