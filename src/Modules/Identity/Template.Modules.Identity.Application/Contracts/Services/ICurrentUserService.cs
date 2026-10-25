namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
    }
}
