using Api.Events.User;

namespace Api.Events
{
    public record AccountFrozen(Guid ProcessedBy);
}