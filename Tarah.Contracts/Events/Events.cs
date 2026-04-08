namespace Tarah.Contracts.Events
{
    public record UserCreated(Guid UserId, string Username);
    public record UserDeleted(Guid UserId);
}
