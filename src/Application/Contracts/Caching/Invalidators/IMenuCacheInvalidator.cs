
namespace Template.Application.Contracts.Caching.Invalidators
{
    // =========================
    // Invalidators
    // =========================

    public interface IMenuCacheInvalidator
    {
        void InvalidateMenu();
        void InvalidatePermissions();
    }
}