namespace Api.Commands
{
    public record WithdrawCommand(Guid AccountId,decimal Amount);
}