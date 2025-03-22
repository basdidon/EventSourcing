namespace Api.Events
{
    public record MoneyDeposited(Guid AccountId, decimal Amount);
}