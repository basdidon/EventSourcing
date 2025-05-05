namespace Api.Features.Accounts.Withdraw
{
    public class Withdrawal
    {
        public Guid RequestId { get; set; }

        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }

        public string Otp { get; set; } = string.Empty;

        public DateTimeOffset ExpiryDate { get; set; }
        public bool IsRevocked { get; set; } = false;
        public int Retry { get; set; }

        public bool IsSuccess { get; set; } = false;

        public Guid CreateBy { get; set; }
    }
}
