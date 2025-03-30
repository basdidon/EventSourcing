namespace Api.Features.Accounts.Withdraw
{
    public class WithdrawalRequest
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }

        public string Otp { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }
        public bool IsRevocked { get; set; } = false;
        public int Retry { get; set; }

        public bool IsSuccess { get; set; } = false;

        public Guid CreateBy { get; set; }
    }
}
