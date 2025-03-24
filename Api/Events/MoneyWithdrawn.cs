using Api.Events.User;

namespace Api.Events
{
    public record MoneyWithdrawn(Guid AccountId,Guid OwnerId, decimal Amount) : UserEvent(OwnerId);
}