namespace Api.Events
{
    public record MoneyTransfered(Guid FromAccountId, Guid SenderId, Guid ToAccountId, Guid RecipientId, decimal Amount);
}