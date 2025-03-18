namespace Api.Commands
{
    public record TransferCommand(Guid FromAccountId,Guid ToAccountId,decimal Amount);
}