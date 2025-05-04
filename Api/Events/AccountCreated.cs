using Api.Events.User;

namespace Api.Events
{
    public record AccountCreated(
        Guid OwnerId,
        Guid CreateBy,
        string AccountNumber,
        decimal InitialBalance
        );
}