using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Template.Core.Application.Utils
{
    public static class CacheUtils
    {
        public static string GenerateETag<TSource>(TSource source)
        {
            // Utiliza un algoritmo de hash, como SHA256, para generar un hash basado en el contenido del producto
            using var sha256 = SHA256.Create();
            var serializedSource = JsonSerializer.Serialize(source);
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(serializedSource));
            var eTag = Convert.ToBase64String(hashBytes);
            return eTag;
        }

        public static void InvalidateCache<TEntity>(IMemoryCache cache)
        {
            string listCacheKey = $"List_{typeof(TEntity).FullName}";
            string cacheKey = $"{typeof(TEntity).FullName}";
            cache.Remove(cacheKey);
            cache.Remove(listCacheKey);
        }
    }
}