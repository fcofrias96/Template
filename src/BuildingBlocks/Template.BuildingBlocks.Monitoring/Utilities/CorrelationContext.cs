namespace Template.BuildingBlocks.Monitoring.Utilities
{
    /// <summary>
    /// Mantiene el CorrelationId accesible en todo el flujo async del request.
    /// </summary>
    public static class CorrelationContext
    {
        private static readonly AsyncLocal<string?> _correlationId = new();

        public static string? CorrelationId
        {
            get => _correlationId.Value;
            set => _correlationId.Value = value;
        }
    }
}
