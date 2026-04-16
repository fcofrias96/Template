
namespace Template.Application.Caching
{
    public sealed class CacheKeyDefinition<TContext>
    {
        public required Func<TContext, object?[]> Parts { get; init; }
        public Func<TContext, string>[] NamespaceKeys { get; init; } = Array.Empty<Func<TContext, string>>();

        public TimeSpan DataTtl { get; init; } = TimeSpan.FromHours(6);
        public TimeSpan NamespaceTtl { get; init; } = TimeSpan.FromDays(7);
    }
}