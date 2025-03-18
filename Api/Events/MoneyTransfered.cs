namespace Api.Events
{
    public record MoneyTransfered(Guid FromAccountId, Guid ToAccountId, decimal Amount);
}