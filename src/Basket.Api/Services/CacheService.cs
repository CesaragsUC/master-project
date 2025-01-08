using Basket.Api.Extensions;
using Basket.Domain.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Api.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        DistributedCacheEntryOptions? cacheOptions = null,
        Func<T, Task>? updateDatabase = null)
    {
        return await _cache.GetOrCreateAsync(key, factory, cacheOptions, updateDatabase);
    }
}
