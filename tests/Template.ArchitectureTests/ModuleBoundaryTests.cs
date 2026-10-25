using System.Reflection;
using Xunit;

namespace Template.ArchitectureTests
{
    /// <summary>
    /// Boundary rules of the modular monolith, checked against the references the compiler actually emitted.
    ///
    /// Modules:        Template.Modules.{Module}.{Contracts|Domain|Application|Infrastructure|Api}
    /// Building blocks: Template.SharedKernel, Template.BuildingBlocks.*
    /// </summary>
    public class ModuleBoundaryTests
    {
        private const string ModulePrefix = "Template.Modules.";
        private const string BuildingBlocksPrefix = "Template.BuildingBlocks.";
        private const string SharedKernel = "Template.SharedKernel";

        private static readonly string[] Modules = ["Catalog", "Identity", "Notifications"];

        private static readonly Lazy<IReadOnlyList<Assembly>> SolutionAssemblies = new(LoadSolutionAssemblies);

        public static TheoryData<string> ModuleAssemblyNames()
        {
            var data = new TheoryData<string>();
            foreach (var assembly in SolutionAssemblies.Value.Where(a => a.GetName().Name!.StartsWith(ModulePrefix)))
            {
                data.Add(assembly.GetName().Name!);
            }

            return data;
        }

        [Fact]
        public void Every_module_is_loaded()
        {
            var loaded = SolutionAssemblies.Value.Select(a => a.GetName().Name).ToHashSet();

            foreach (var module in Modules)
            {
                Assert.Contains(loaded, name => name!.StartsWith($"{ModulePrefix}{module}."));
            }
        }

        [Theory]
        [MemberData(nameof(ModuleAssemblyNames))]
        public void Modules_only_reference_other_modules_through_their_contracts(string assemblyName)
        {
            var owner = ModuleOf(assemblyName);

            var violations = ReferencesOf(assemblyName)
                .Where(reference => reference.StartsWith(ModulePrefix))
                .Where(reference => ModuleOf(reference) != owner)
                .Where(reference => !reference.EndsWith(".Contracts"))
                .ToList();

            Assert.True(violations.Count == 0,
                $"{assemblyName} must reach other modules only via *.Contracts, but references: {string.Join(", ", violations)}");
        }

        [Theory]
        [MemberData(nameof(ModuleAssemblyNames))]
        public void Contracts_do_not_depend_on_module_internals(string assemblyName)
        {
            if (!assemblyName.EndsWith(".Contracts"))
            {
                return;
            }

            var violations = ReferencesOf(assemblyName)
                .Where(reference => reference.StartsWith(ModulePrefix) || reference.StartsWith(BuildingBlocksPrefix))
                .ToList();

            Assert.True(violations.Count == 0,
                $"{assemblyName} must stay self-contained, but references: {string.Join(", ", violations)}");
        }

        [Theory]
        [MemberData(nameof(ModuleAssemblyNames))]
        public void Domain_has_no_application_infrastructure_or_web_dependencies(string assemblyName)
        {
            if (!assemblyName.EndsWith(".Domain"))
            {
                return;
            }

            var violations = ReferencesOf(assemblyName)
                .Where(reference => reference.StartsWith(BuildingBlocksPrefix)
                                    || reference.StartsWith("Microsoft.EntityFrameworkCore")
                                    || reference.StartsWith("Microsoft.AspNetCore.Mvc")
                                    || (reference.StartsWith(ModulePrefix) && !reference.EndsWith(".Contracts")))
                .ToList();

            Assert.True(violations.Count == 0,
                $"{assemblyName} must only depend on the shared kernel and contracts, but references: {string.Join(", ", violations)}");
        }

        [Theory]
        [MemberData(nameof(ModuleAssemblyNames))]
        public void Application_does_not_depend_on_infrastructure_or_api(string assemblyName)
        {
            if (!assemblyName.EndsWith(".Application"))
            {
                return;
            }

            var violations = ReferencesOf(assemblyName)
                .Where(reference => reference.EndsWith(".Infrastructure")
                                    || reference.EndsWith(".Api")
                                    || reference == $"{BuildingBlocksPrefix}Web"
                                    || reference.StartsWith("Microsoft.EntityFrameworkCore"))
                .ToList();

            Assert.True(violations.Count == 0,
                $"{assemblyName} must not depend on infrastructure, web or EF Core, but references: {string.Join(", ", violations)}");
        }

        [Fact]
        public void Building_blocks_do_not_depend_on_modules()
        {
            var violations = SolutionAssemblies.Value
                .Select(a => a.GetName().Name!)
                .Where(name => name.StartsWith(BuildingBlocksPrefix) || name == SharedKernel)
                .SelectMany(name => ReferencesOf(name)
                    .Where(reference => reference.StartsWith(ModulePrefix))
                    .Select(reference => $"{name} -> {reference}"))
                .ToList();

            Assert.True(violations.Count == 0,
                $"Building blocks must be module-agnostic, but found: {string.Join(", ", violations)}");
        }

        [Fact]
        public void Shared_kernel_has_no_dependencies_on_the_solution()
        {
            var violations = ReferencesOf(SharedKernel)
                .Where(reference => reference.StartsWith("Template."))
                .ToList();

            Assert.Empty(violations);
        }

        private static string ModuleOf(string assemblyName)
            => assemblyName[ModulePrefix.Length..].Split('.')[0];

        private static IReadOnlyList<string> ReferencesOf(string assemblyName)
            => SolutionAssemblies.Value
                .Single(a => a.GetName().Name == assemblyName)
                .GetReferencedAssemblies()
                .Select(reference => reference.Name!)
                .ToList();

        private static IReadOnlyList<Assembly> LoadSolutionAssemblies()
        {
            var directory = AppContext.BaseDirectory;

            return Directory.GetFiles(directory, "Template.*.dll")
                .Concat(Directory.GetFiles(directory, $"{SharedKernel}.dll"))
                .Distinct()
                .Select(path => Assembly.LoadFrom(path))
                .Where(a => a.GetName().Name!.StartsWith(ModulePrefix)
                            || a.GetName().Name!.StartsWith(BuildingBlocksPrefix)
                            || a.GetName().Name == SharedKernel)
                .ToList();
        }
    }
}
