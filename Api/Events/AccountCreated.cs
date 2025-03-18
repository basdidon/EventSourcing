namespace Api.Events
{
    public record AccountCreated(Guid AccountId, string AccountNumber, decimal InitialBalance);
}