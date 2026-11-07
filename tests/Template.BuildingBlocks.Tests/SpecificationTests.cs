using Template.BuildingBlocks.Application.Attributes;
using Template.BuildingBlocks.Application.Models.Specification;
using Template.BuildingBlocks.Application.Utilities;
using Template.SharedKernel.Domain;
using Xunit;

namespace Template.BuildingBlocks.Tests
{
    public class SpecificationTests
    {
        [Fact]
        public void Valid_parameters_build_paging_sorting_and_criteria()
        {
            var result = Specification<Product>.Create(new ProductQuery { Offset = 20, Limit = 10, SortBy = "Name", Name = "lap", Quantity = "5" });

            Assert.True(result.IsSuccess);
            var specification = result.Value;
            Assert.Equal(20, specification.Skip);
            Assert.Equal(10, specification.Take);

            var matches = specification.Criteria!.Compile();
            Assert.True(matches(new Product { Name = "laptop", Quantity = 5 }));
            Assert.False(matches(new Product { Name = "laptop", Quantity = 6 }));
            Assert.False(matches(new Product { Name = "phone", Quantity = 5 }));
        }

        [Fact]
        public void Pagination_is_skipped_when_not_requested()
        {
            var result = Specification<Product>.Create(new ProductQuery { Offset = 20, Limit = 10 }, applyPagination: false);

            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value.Skip);
            Assert.Equal(0, result.Value.Take);
        }

        [Fact]
        public void A_navigation_sort_field_defaults_to_its_Name()
        {
            var result = Specification<Product>.Create(new ProductQuery { SortBy = "Category" });

            Assert.True(result.IsSuccess);
            var sortKey = result.Value.SortBy.Compile()(new Product { Category = new Category { Name = "Tools" } });
            Assert.Equal("Tools", sortKey);
        }

        [Theory]
        [InlineData("NoSuchField", "NoSuchField")]
        [InlineData("Category.NoSuchField", "NoSuchField")]
        public void An_unknown_sort_field_is_a_validation_error(string sortBy, string reported)
        {
            var result = Specification<Product>.Create(new ProductQuery { SortBy = sortBy });

            Assert.True(result.IsFailure);
            Assert.Equal("Query.UnknownProperty", result.Error.Code);
            Assert.Contains($"'{reported}'", result.Error.Description);
        }

        [Fact]
        public void A_blank_sort_field_is_a_validation_error()
        {
            var result = Specification<Product>.Create(new ProductQuery { SortBy = "  " });

            Assert.True(result.IsFailure);
            Assert.Equal("Query.SortByRequired", result.Error.Code);
        }

        [Theory]
        [InlineData("abc", null)]       // not a number
        [InlineData(null, "1..abc")]    // bad upper bound of a range
        [InlineData(null, "abc..5")]    // bad lower bound of a range
        public void An_unparsable_filter_value_is_a_validation_error(string? quantity, string? price)
        {
            var result = Specification<Product>.Create(new ProductQuery { Quantity = quantity, Price = price });

            Assert.True(result.IsFailure);
            Assert.Equal("Query.InvalidValue", result.Error.Code);
            Assert.Contains("abc", result.Error.Description);
        }

        [Fact]
        public void An_open_ended_range_keeps_only_the_given_bound()
        {
            var result = Specification<Product>.Create(new ProductQuery { Price = "10.." });

            Assert.True(result.IsSuccess);
            var matches = result.Value.Criteria!.Compile();
            Assert.True(matches(new Product { Price = 10m }));
            Assert.True(matches(new Product { Price = 999m }));
            Assert.False(matches(new Product { Price = 9.99m }));
        }

        private sealed class Category
        {
            public string Name { get; set; } = string.Empty;
        }

        private sealed class Product : EntityBase
        {
            public string Name { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public Category Category { get; set; } = new();
        }

        private sealed class ProductQuery : QueryParametersBase<Product>
        {
            public override string? SortBy { get; set; }
            public string? Name { get; set; }
            public string? Quantity { get; set; }

            [NumberRangeFormat]
            public string? Price { get; set; }
        }
    }
}
