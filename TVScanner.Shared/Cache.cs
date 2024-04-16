
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace TVScanner.Shared
{
    public interface ICache
    {
        T Get<T>(string key);
        Task Set<T>(string key, T value);
        void Subscribe<T>(string relativeVolume, Func<object, Task> value);
    }

    public class InMemoryCache : ICache
    {
        private readonly Dictionary<string, List<Func<object, Task>>> _subscribers = new Dictionary<string, List<Func<object, Task>>>();
        private MemoryCacheEntryOptions _cacheEntryOptions;
        private readonly IMemoryCache _cache;

        public InMemoryCache(IMemoryCache memoryCache)
        {
            // need a mechanism to clear the cache after a certain period of time
            _cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache = memoryCache;
        }

        /// <summary>
        /// whenever a message is published, the subscriber's method will be called
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageName">The name of the message</param>
        /// <param name="value">the value of the message</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Subscribe<T>(string messageName, Func<object, Task> value)
        {
            if (!_subscribers.ContainsKey(messageName))
            {
                _subscribers[messageName] = new List<Func<object, Task>>();
            }

            _subscribers[messageName].Add(value);
        }

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out string? data))
            {
                return JsonSerializer.Deserialize<T>(data!.ToString())!;
            }

            return default;
        }

        public async Task Set<T>(string key, T value)
        {
            _cache.Set(key, JsonSerializer.Serialize(value), _cacheEntryOptions);

            if (_subscribers.ContainsKey(key))
            {
                foreach (var subscriber in _subscribers[key])
                {
                    await subscriber(value!);
                }
            }
        }
    }
}
