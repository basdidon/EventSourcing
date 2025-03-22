namespace Api.Events
{
    public record AccountCreated(Guid AccountId,Guid Owner, string AccountNumber, decimal InitialBalance);
}