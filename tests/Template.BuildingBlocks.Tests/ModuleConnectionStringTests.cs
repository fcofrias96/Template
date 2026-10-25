using Microsoft.Extensions.Configuration;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Xunit;

namespace Template.BuildingBlocks.Tests
{
    public class ModuleConnectionStringTests
    {
        [Fact]
        public void Falls_back_to_the_default_connection_string()
        {
            var configuration = Build(("Default", "shared-db"));

            Assert.Equal("shared-db", configuration.GetModuleConnectionString("Catalog"));
        }

        [Fact]
        public void Module_connection_string_overrides_the_default()
        {
            var configuration = Build(("Default", "shared-db"), ("Identity", "identity-db"));

            Assert.Equal("identity-db", configuration.GetModuleConnectionString("Identity"));
            Assert.Equal("shared-db", configuration.GetModuleConnectionString("Catalog"));
        }

        [Fact]
        public void Throws_when_neither_module_nor_default_is_configured()
        {
            var configuration = Build();

            var exception = Assert.Throws<InvalidOperationException>(() => configuration.GetModuleConnectionString("Audit"));
            Assert.Contains("Audit", exception.Message);
            Assert.Contains("Default", exception.Message);
        }

        private static IConfiguration Build(params (string Name, string Value)[] connectionStrings)
            => new ConfigurationBuilder()
                .AddInMemoryCollection(connectionStrings.Select(c =>
                    new KeyValuePair<string, string?>($"ConnectionStrings:{c.Name}", c.Value)))
                .Build();
    }
}
