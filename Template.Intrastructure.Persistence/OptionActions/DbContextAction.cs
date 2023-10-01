using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Template.Infrastructure.Persistence.DbContexts;

namespace Template.Infrastructure.Persistence.OptionActions
{
    public static class DbContextAction
    {
        public static readonly Func<IConfiguration, Action<DbContextOptionsBuilder>> DbOptions = configuration =>
        {
            Action<DbContextOptionsBuilder> Options = (options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                            optionAction => optionAction.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

                options.EnableSensitiveDataLogging();
            };

            return Options;
        };

        public static readonly Func<IConfiguration, Action<DbContextOptionsBuilder>> AuditDbOptions = configuration =>
        {
            Action<DbContextOptionsBuilder> Options = (options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("AuditConnection"),
                            optionAction => optionAction.MigrationsAssembly(typeof(AuditDbContext).Assembly.FullName));

                options.EnableSensitiveDataLogging();
            };

            return Options;
        };

        public static readonly Action<DbContextOptionsBuilder> InMemoryOptions = (options) =>
        {
            options.UseInMemoryDatabase("ApplicationDb");
        };

        public static readonly Action<DbContextOptionsBuilder> AuditInMemoryOptions = (options) =>
        {
            options.UseInMemoryDatabase("ApplicationDb");
        };
    }
}