using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Template.BuildingBlocks.Application.Caching;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.BuildingBlocks.Web.Extensions;
using Template.BuildingBlocks.Web.Results;
using Template.SharedKernel.Results;
using Xunit;

namespace Template.BuildingBlocks.Tests
{
    public class ResultTests
    {
        private static readonly Error SomeError = Error.NotFound("Thing.NotFound", "Thing was not found");

        [Fact]
        public void Success_carries_the_value_and_no_error()
        {
            Result<int> result = Result.Success(42);

            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(42, result.Value);
            Assert.Equal(Error.None, result.Error);
        }

        [Fact]
        public void An_error_converts_implicitly_to_a_failure()
        {
            Result<int> typed = SomeError;
            Result untyped = SomeError;

            Assert.True(typed.IsFailure);
            Assert.Equal(SomeError, typed.Error);
            Assert.True(untyped.IsFailure);
            Assert.Equal(SomeError, untyped.Error);
        }

        [Fact]
        public void Reading_the_value_of_a_failure_throws()
        {
            Result<int> result = SomeError;

            Assert.Throws<InvalidOperationException>(() => result.Value);
        }

        [Fact]
        public void A_failure_must_carry_a_real_error()
        {
            Assert.Throws<ArgumentException>(() => Result.Failure(Error.None));
        }

        [Fact]
        public void ToNoContent_maps_success_to_204_and_failure_to_the_error()
        {
            Assert.IsType<NoContentResult>(Result.Success().ToNoContent());

            var failure = Assert.IsType<ErrorResult>(Result.Failure(SomeError).ToNoContent());
            Assert.Equal(SomeError, failure.Error);
        }

        [Fact]
        public void ToOk_wraps_the_value_in_an_ApiResponse()
        {
            var ok = Assert.IsType<OkObjectResult>(Result.Success("value").ToOk());

            var body = Assert.IsType<ApiResponse<string>>(ok.Value);
            Assert.Equal("value", body.Data);
            Assert.True(body.Success);
        }

        [Fact]
        public void ToOk_on_a_failure_does_not_run_the_success_branch()
        {
            Result<string> result = SomeError;

            Assert.IsType<ErrorResult>(result.ToOk());
        }

        [Fact]
        public async Task A_null_cached_by_a_factory_is_not_served_as_a_hit()
        {
            // BaseService.GetByIdProjectedWithEtagAsync relies on this: a missing row yields a null entry,
            // which must be looked up again instead of being returned from the cache.
            var cache = new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()));
            var calls = 0;

            Task<string?> Factory(CancellationToken _)
            {
                calls++;
                return Task.FromResult<string?>(null);
            }

            Assert.Null(await cache.GetOrCreateAsync("missing", TimeSpan.FromMinutes(5), Factory));
            Assert.Null(await cache.GetOrCreateAsync("missing", TimeSpan.FromMinutes(5), Factory));
            Assert.Equal(2, calls);
        }
    }
}
