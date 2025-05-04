namespace Api.Events
{
    public record MoneyReceived(Guid TransactionId, decimal Amount);
}