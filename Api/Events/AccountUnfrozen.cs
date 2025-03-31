using Api.Events.User;

namespace Api.Events
{
    public record AccountUnfrozen(Guid AccountId,Guid OwnerId,Guid ProcessedBy):UserEvent(OwnerId);
}