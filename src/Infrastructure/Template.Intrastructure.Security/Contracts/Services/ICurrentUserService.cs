namespace Template.Security.Contracts.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
    }
}