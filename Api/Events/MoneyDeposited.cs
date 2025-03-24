using Api.Events.User;

namespace Api.Events
{
    public record MoneyDeposited(Guid AccountId,Guid OwnerId, decimal Amount):UserEvent(OwnerId);
}