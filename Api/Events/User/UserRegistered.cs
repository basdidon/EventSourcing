namespace Api.Events.User
{
    public class User
    {
        public Guid Id { get; set; }
    }

    public record UserEvent(Guid UserId);
    public record UserRegistered(Guid UserId):UserEvent(UserId);
}
