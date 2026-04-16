namespace Template.Application.Contracts.Caching.Invalidators
{
    public interface IUserAuthCacheInvalidator
    {
        void InvalidateAll();
        void InvalidateUser(int userId);
    }
}