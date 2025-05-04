using Api.Events.User;

namespace Api.Events
{
    public record MoneyDeposited(
        decimal Amount,
        Guid ProcessedBy
        );
}