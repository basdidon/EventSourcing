using Api.Events.User;

namespace Api.Events
{
    public record AccountCreated(
        Guid AccountId,
        Guid CreateBy,
        Guid OwnerId,
        string AccountNumber,
        decimal InitialBalance
        ) : UserEvent(OwnerId);
}