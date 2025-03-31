using Api.Events.User;

namespace Api.Events
{
    public record AccountFrozen(Guid AccountId,Guid OwnerId,Guid ProcessedBy):UserEvent(OwnerId);
}