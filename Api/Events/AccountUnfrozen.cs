using Api.Events.User;

namespace Api.Events
{
    public record AccountUnfrozen(Guid ProcessedBy);
}