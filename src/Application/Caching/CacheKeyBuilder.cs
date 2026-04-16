using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Template.Application.Caching
{
    /// <summary>
    /// ✅ Builder robusto:
    /// - Escapa separador ":" para evitar colisiones
    /// - Normaliza tipos con cultura Invariant
    /// - Si la key queda muy larga, agrega hash SHA256
    /// </summary>
    public sealed class CacheKeyBuilder : ICacheKeyBuilder
    {
        // Ajusta según tu backend (Redis suele tolerar muy largo, pero conviene limitar).
        private const int MaxKeyLen = 300;

        public string Build(params object?[] parts)
        {
            var normalized = parts.Select(NormalizePart);
            var key = string.Join(":", normalized);

            if (key.Length > MaxKeyLen)
            {
                var hash = Sha256Hex(key);
                // Mantén un prefijo pequeño para debugging, y un hash para unicidad
                var prefixLen = Math.Min(80, key.Length);
                key = $"{key[..prefixLen]}:sha256:{hash}";
            }

            return key;
        }

        private static string NormalizePart(object? value)
        {
            if (value is null) return "null";

            return value switch
            {
                string s => Escape(s),
                Guid g => g.ToString("N"),
                DateTime dt => dt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                DateTimeOffset dto => dto.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                bool b => b ? "1" : "0",
                IFormattable f => Escape(f.ToString(null, CultureInfo.InvariantCulture) ?? "null"),
                _ => Escape(value.ToString() ?? "null")
            };
        }

        private static string Escape(string s)
            => s.Replace("%", "%25").Replace(":", "%3A");

        private static string Sha256Hex(string input)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
    }
}