using Api.Events.User;

namespace Api.Events
{
    public record AccountClosed(Guid AccountId,Guid OwnerId):UserEvent(OwnerId);
}