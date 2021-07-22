using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Redis.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordId,T data, TimeSpan? absolutExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            //Give us a default time in case of being null
            options.AbsoluteExpirationRelativeToNow = absolutExpireTime ?? TimeSpan.FromSeconds(60);
            options.SlidingExpiration = unusedExpireTime;

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData, options);
        }

        public static async Task<T> GetRecordAsync<T> (this IDistributedCache cache,string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

            if(jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
