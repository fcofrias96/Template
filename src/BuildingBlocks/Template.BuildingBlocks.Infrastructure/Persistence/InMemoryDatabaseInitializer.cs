using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Template.BuildingBlocks.Infrastructure.Persistence
{
    /// <summary>
    /// The EF Core in-memory provider only applies <c>HasData</c> seed data when the database is created,
    /// so each module's DbContext is created once at startup when <c>UseInMemoryDatabase</c> is enabled.
    /// </summary>
    public sealed class InMemoryDatabaseInitializer<TContext> : IHostedService
        where TContext : DbContext
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public InMemoryDatabaseInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public static class InMemoryDatabaseInitializerExtensions
    {
        public static bool UseInMemoryDatabase(this IConfiguration configuration)
            => configuration.GetValue<bool>("UseInMemoryDatabase");

        /// <summary>
        /// Registers <see cref="InMemoryDatabaseInitializer{TContext}"/> when the in-memory database is enabled.
        /// </summary>
        public static IServiceCollection AddInMemoryDatabaseInitializer<TContext>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
        {
            if (configuration.UseInMemoryDatabase())
            {
                services.AddHostedService<InMemoryDatabaseInitializer<TContext>>();
            }

            return services;
        }
    }
}
