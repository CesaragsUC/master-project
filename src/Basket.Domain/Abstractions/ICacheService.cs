using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Domain.Abstractions;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
    string key,
    Func<Task<T>> factory,
    DistributedCacheEntryOptions? cacheOptions = null,
    Func<T, Task>? updateDatabase = null);
}
