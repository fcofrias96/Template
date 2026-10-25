using Microsoft.Extensions.Caching.Memory;
using Template.BuildingBlocks.Application.Caching;
using Xunit;

namespace Template.BuildingBlocks.Tests
{
    public class MemoryCacheServiceTests
    {
        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

        [Fact]
        public void Set_then_get_returns_the_value()
        {
            var cache = CreateCache();

            cache.Set("key", 42, Ttl);

            Assert.Equal(42, cache.Get<int>("key"));
            Assert.Equal(42, cache.Get<int?>("key"));
        }

        [Fact]
        public void Get_returns_default_for_missing_key_or_other_type()
        {
            var cache = CreateCache();
            cache.Set("key", "text", Ttl);

            Assert.Null(cache.Get<string>("missing"));
            Assert.Null(cache.Get<int?>("key"));
        }

        [Fact]
        public async Task GetOrCreate_runs_the_factory_only_on_a_miss()
        {
            var cache = CreateCache();
            var calls = 0;

            Task<int> Factory(CancellationToken _) => Task.FromResult(++calls);

            Assert.Equal(1, await cache.GetOrCreateAsync("key", Ttl, Factory));
            Assert.Equal(1, await cache.GetOrCreateAsync("key", Ttl, Factory));
            Assert.Equal(1, calls);
        }

        [Fact]
        public void Remove_evicts_a_single_key()
        {
            var cache = CreateCache();
            cache.Set("a", 1, Ttl, region: "r");
            cache.Set("b", 2, Ttl, region: "r");

            cache.Remove("a");

            Assert.Equal(0, cache.Get<int>("a"));
            Assert.Equal(2, cache.Get<int>("b"));
        }

        [Fact]
        public async Task RemoveRegion_evicts_only_the_entries_of_that_region()
        {
            var cache = CreateCache();
            cache.Set("category:list", "categories", Ttl, region: "Category");
            await cache.GetOrCreateAsync("category:page", Ttl, _ => Task.FromResult("page"), region: "Category");
            cache.Set("item:list", "items", Ttl, region: "Item");
            cache.Set("no-region", "kept", Ttl);

            cache.RemoveRegion("Category");

            Assert.Null(cache.Get<string>("category:list"));
            Assert.Null(cache.Get<string>("category:page"));
            Assert.Equal("items", cache.Get<string>("item:list"));
            Assert.Equal("kept", cache.Get<string>("no-region"));
        }

        [Fact]
        public void Region_can_be_reused_after_it_was_removed()
        {
            var cache = CreateCache();
            cache.Set("key", "old", Ttl, region: "r");
            cache.RemoveRegion("r");

            cache.Set("key", "new", Ttl, region: "r");

            Assert.Equal("new", cache.Get<string>("key"));
        }

        [Fact]
        public void Removing_an_unknown_region_is_a_no_op()
        {
            var cache = CreateCache();

            cache.RemoveRegion("never-used");
        }

        [Fact]
        public async Task Concurrent_misses_on_the_same_key_run_the_factory_once()
        {
            var cache = CreateCache();
            var calls = 0;
            var release = new TaskCompletionSource();

            async Task<int> SlowFactory(CancellationToken _)
            {
                Interlocked.Increment(ref calls);
                await release.Task;
                return 42;
            }

            var callers = Enumerable.Range(0, 20)
                .Select(_ => Task.Run(() => cache.GetOrCreateAsync("key", Ttl, SlowFactory)))
                .ToArray();

            await Task.Delay(100);
            release.SetResult();
            var results = await Task.WhenAll(callers);

            Assert.Equal(1, calls);
            Assert.All(results, result => Assert.Equal(42, result));
            Assert.Equal(0, cache.PendingKeyLocks);
        }

        [Fact]
        public async Task Different_keys_load_in_parallel()
        {
            var cache = CreateCache();
            var bothStarted = new TaskCompletionSource();
            var started = 0;

            async Task<string> Factory(CancellationToken _)
            {
                if (Interlocked.Increment(ref started) == 2)
                {
                    bothStarted.SetResult();
                }

                // Completes only if the other key's factory is running at the same time.
                await bothStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
                return "value";
            }

            await Task.WhenAll(
                cache.GetOrCreateAsync("a", Ttl, Factory),
                cache.GetOrCreateAsync("b", Ttl, Factory));

            Assert.Equal(2, started);
        }

        [Fact]
        public async Task A_failing_factory_releases_the_lock_and_the_next_caller_retries()
        {
            var cache = CreateCache();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                cache.GetOrCreateAsync<int>("key", Ttl, _ => throw new InvalidOperationException("db down")));

            Assert.Equal(7, await cache.GetOrCreateAsync("key", Ttl, _ => Task.FromResult(7)));
            Assert.Equal(0, cache.PendingKeyLocks);
        }

        [Fact]
        public async Task A_cancelled_waiter_does_not_block_or_leak_the_lock()
        {
            var cache = CreateCache();
            var release = new TaskCompletionSource();
            var loading = cache.GetOrCreateAsync("key", Ttl, async _ => { await release.Task; return 1; });

            using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                cache.GetOrCreateAsync("key", Ttl, _ => Task.FromResult(2), cancellationToken: cancellation.Token));

            release.SetResult();
            Assert.Equal(1, await loading);
            Assert.Equal(0, cache.PendingKeyLocks);
        }

        private static MemoryCacheService CreateCache() => new(new MemoryCache(new MemoryCacheOptions()));
    }
}
