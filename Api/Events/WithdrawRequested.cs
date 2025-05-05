namespace Api.Events
{
    public record WithdrawRequested(Guid AccountId, decimal Amount, string Otp, DateTimeOffset ExpiryDate,int Retry,Guid CreateBy);
}
