namespace Tarah.Contracts.Events
{
    public record UserCreated(Guid UserId);
    public record UserDeleted(Guid UserId);
}
