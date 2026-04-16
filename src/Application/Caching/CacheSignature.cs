using System.Security.Cryptography;
using System.Text;

namespace Template.Application.Caching
{
    // =========================
    // Helpers (hash de firmas)
    // =========================

    public static class CacheSignature
    {
        /// <summary>
        /// Hash corto (SHA256 hex) de una firma potencialmente grande (permisos, filtros).
        /// </summary>
        public static string Hash(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "none";

            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            // hex completo (64 chars). Si quieres más corto, corta a 16/24/32 chars.
            return Convert.ToHexString(bytes);
        }

        /// <summary>
        /// Hash corto de 16 chars (para keys más pequeñas).
        /// </summary>
        public static string Hash16(string? value)
        {
            var hex = Hash(value);
            return hex.Length <= 16 ? hex : hex[..16];
        }
    }
}