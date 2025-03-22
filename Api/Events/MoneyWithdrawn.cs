namespace Api.Events
{
    public record MoneyWithdrawn(Guid AccountId, decimal Amount);
}