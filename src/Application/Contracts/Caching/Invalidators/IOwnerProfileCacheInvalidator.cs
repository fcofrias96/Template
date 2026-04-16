namespace Template.Application.Contracts.Caching.Invalidators
{
    public interface IOwnerProfileCacheInvalidator
    {
        void InvalidateAll();
        void InvalidateUser(int userId);
    }
}