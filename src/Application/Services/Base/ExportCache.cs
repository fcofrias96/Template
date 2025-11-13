using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public partial class ExportServiceAsync<TEntity, TDbContext> where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        #region Private Methods

        /// <summary>
        /// Cache por tipo de destino para evitar reflexión repetida.
        /// </summary>
        private static class ExportCache<T>
        {
            public static readonly PropertyInfo[] Properties;
            public static readonly string[] Headers;

            static ExportCache()
            {
                Properties = typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                Headers = Properties
                    .Select(GetDisplayName)
                    .ToArray();
            }
        }

        #endregion Private Methods
    }
}
