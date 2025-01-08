using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Basket.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class DistributedCacheExtensions
{
    public static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),

        //should always be set lower than the absolute expiration
        SlidingExpiration = TimeSpan.FromMinutes(2)
    };

    public static async Task<T> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T>> factory,
        DistributedCacheEntryOptions? cacheOptions = null,
        Func<T, Task> updateDatabase = null)
    {
        // 1. Obter os dados do cache
        var cachedData = await cache.GetStringAsync(key);

        // 2. Executar o factory para obter o estado mais recente
        var newData = await factory();
        var serializedNewData = JsonSerializer.Serialize(newData);

        // 3. Sincronizar o MongoDB (garante consistência com o estado mais recente)
        if (updateDatabase != null)
        {
            await updateDatabase(newData);
        }

        // 4. Verificar se o cache está igual ao novo estado
        if (cachedData is not null && await AreObjectsEqualAsync(cache,cachedData, serializedNewData))
        {
            // Se o cache é igual, retorna os dados do cache
            return JsonSerializer.Deserialize<T>(cachedData);
        }

        // 5. Atualizar o cache com os novos dados
        await cache.SetStringAsync(
            key,
            serializedNewData,
            cacheOptions ?? DefaultExpiration);

        // 6. Retornar os novos dados
        return newData;
    }

    public static async Task<bool> AreObjectsEqualAsync(IDistributedCache cache, string obj1, string obj2)
    {
        if (string.IsNullOrEmpty(obj1) || string.IsNullOrEmpty(obj2))
        {
            return false; 
        }

        // Usa Newtonsoft.Json para comparar os objetos ignorando a ordem das propriedades
        return JToken.DeepEquals(JToken.Parse(obj1), JToken.Parse(obj2));
    }
}
