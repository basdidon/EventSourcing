namespace Api.Entities
{
    public class BankAccount
    {
        public Guid Id { get; set; }

        public string AccountNumber { get; set; } = string.Empty;  // xxx-xxxxxx-x

        public decimal Balance { get; set; }
    }

    public class TransferRecord
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public decimal Amount { get; set; }
    }
}