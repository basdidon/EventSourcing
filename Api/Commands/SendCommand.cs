namespace Api.Commands
{
    public record SendCommand(Guid ToAccountId, decimal Amount);
}