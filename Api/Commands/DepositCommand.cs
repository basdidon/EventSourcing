namespace Api.Commands
{
    public record DepositCommand(Guid AccountId, decimal Amount);
}
