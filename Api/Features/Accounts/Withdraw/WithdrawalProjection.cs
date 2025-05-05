using Api.Events;
using Marten.Events.Aggregation;

namespace Api.Features.Accounts.Withdraw
{
    public class WithdrawalProjection : SingleStreamProjection<Withdrawal>
    {
        public static Withdrawal Create(WithdrawRequested e)
        => new()
        {
            AccountId = e.AccountId,
            Amount = e.Amount,
            Otp = e.Otp,
            ExpiryDate = e.ExpiryDate,
            Retry = e.Retry,
            CreateBy = e.CreateBy,
        };

        public static void Apply(WithdrawConfirmed _,Withdrawal v) => v.IsSuccess = true;
        public static void Apply(WithdrawRejected _, Withdrawal v) => v.Retry--;
        public static void Apply(WithdrawRevocked _, Withdrawal v) => v.IsRevocked = true;
    }
}
