using System.Diagnostics.Metrics;

namespace Template.Monitoring.Utilities
{
    public static class ObservabilityMetrics
    {
        // Nombre del "meter" (identifica las métricas de tu servicio)
        public const string MeterName = "Template.Api";

        private static readonly Meter _meter = new(MeterName);

        /// <summary>
        /// Duración de requests HTTP en milisegundos.
        /// </summary>
        public static readonly Histogram<double> RequestDurationMs =
            _meter.CreateHistogram<double>(
                name: "http.server.request.duration",
                unit: "ms",
                description: "Duración de requests HTTP en milisegundos");

        /// <summary>
        /// Duración de comandos de base de datos en milisegundos.
        /// </summary>
        public static readonly Histogram<double> DbCommandDurationMs =
            _meter.CreateHistogram<double>(
                name: "db.command.duration",
                unit: "ms",
                description: "Duración de comandos de base de datos en milisegundos");
    }
}
