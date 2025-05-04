using Api.Events.User;

namespace Api.Events
{
    public record MoneyWithdrawn(decimal Amount,Guid ProcessedBy);
}