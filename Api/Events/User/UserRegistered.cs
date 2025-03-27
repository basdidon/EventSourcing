namespace Api.Events.User
{
    public record UserRegistered(Guid UserId):UserEvent(UserId);
}
