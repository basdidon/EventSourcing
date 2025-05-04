namespace Api.Events
{
    public record MoneySent(Guid TransactionId, decimal Amount);
}