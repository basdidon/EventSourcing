namespace Api.Events
{
    public record AccountCreated(Guid AccountId,Guid OwnerId, string AccountNumber, decimal InitialBalance);
}