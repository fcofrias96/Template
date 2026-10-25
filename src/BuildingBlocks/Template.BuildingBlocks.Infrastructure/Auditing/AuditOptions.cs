namespace Template.BuildingBlocks.Infrastructure.Auditing
{
    /// <summary>
    /// Audit settings contributed by modules, e.g. <c>services.Configure&lt;AuditOptions&gt;(o =&gt; o.Mask&lt;User&gt;(...))</c>.
    /// </summary>
    public sealed class AuditOptions
    {
        public const string MaskedValue = "***";

        private readonly HashSet<(Type EntityType, string Property)> _maskedProperties = new();

        /// <summary>
        /// Records that the properties changed but never their values (secrets, hashes, tokens).
        /// Applies to <typeparamref name="TEntity"/> and its subclasses.
        /// </summary>
        public AuditOptions Mask<TEntity>(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                _maskedProperties.Add((typeof(TEntity), propertyName));
            }

            return this;
        }

        public bool IsMasked(Type entityType, string propertyName)
        {
            for (var type = entityType; type is not null; type = type.BaseType)
            {
                if (_maskedProperties.Contains((type, propertyName)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
