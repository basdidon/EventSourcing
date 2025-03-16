namespace EventSourcing.Commands
{
    public record CreateMovie(Guid Id,string Name, DateTime ReleaseDate);
}