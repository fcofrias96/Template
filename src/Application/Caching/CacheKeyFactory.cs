using Template.Application.Caching.Models;

namespace Template.Application.Caching
{
    /// <summary>
    /// ✅ Factory:
    /// - Construye la key base
    /// - Encadena namespaces en orden fijo
    /// - Valida NamespaceTtl >= DataTtl para evitar "revivir" keys viejas
    /// </summary>
    public sealed class CacheKeyFactory : ICacheKeyFactory
    {
        private readonly ICacheKeyBuilder _builder;
        private readonly ICacheNamespaceVersion _ns;

        public CacheKeyFactory(ICacheKeyBuilder builder, ICacheNamespaceVersion ns)
        {
            _builder = builder;
            _ns = ns;
        }

        public CacheKeyResult Build<TContext>(CacheKeyDefinition<TContext> def, TContext ctx)
        {
            if (def.NamespaceTtl < def.DataTtl)
            {
                throw new InvalidOperationException(
                    $"NamespaceTtl ({def.NamespaceTtl}) should be >= DataTtl ({def.DataTtl}).");
            }

            // key base
            var key = _builder.Build(def.Parts(ctx));

            // aplica N namespaces en cadena (global -> por entidad -> por usuario -> etc.)
            // ✅ Mantén el orden estable en el registro de policies
            foreach (var nsKeyFn in def.NamespaceKeys)
            {
                var nsKey = nsKeyFn(ctx);
                var v = _ns.GetOrInit(nsKey, def.NamespaceTtl);
                key = _builder.Build(key, "v", v);
            }

            return new CacheKeyResult(key, def.DataTtl);
        }
    }
}