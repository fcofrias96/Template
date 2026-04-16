
namespace Template.Application.Contracts.Caching.Invalidators
{
    public interface IPermissionCacheInvalidator
    {
        void InvalidateAll();            // cambios globales de permisos
        void InvalidateUser(int userId); // cambia permisos efectivos de 1 user
    }
}