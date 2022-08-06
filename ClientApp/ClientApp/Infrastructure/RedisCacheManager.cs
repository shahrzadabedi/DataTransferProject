using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class RedisCacheManager : ICacheManager
    {
        protected readonly IDatabase _redisDB;
        protected ISerializer _serializer;
        public RedisCacheManager(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
        {
            _redisDB = connectionMultiplexer.GetDatabase();
            _serializer = serializer;
        }

        public async Task<IEnumerable<TDto>> ReadAllData<TDto>() where TDto : class
        {
            string dtoString = typeof(TDto).ToString();
            int dtoIndex = dtoString.IndexOf("Dto");            
            string keyPrefix = dtoString.Remove(dtoIndex); ;
            var keyList = ListAllKeys(keyPrefix);
            RedisValue[] cacheValues = await GetAllCacheValues(keyList);
            var list = cacheValues.ToList().Select(p => p.ToString()).ToArray();
            var dtoList = _serializer.DeserializeAllData<TDto>(list);
            return dtoList;
        }

        public async Task CacheAllData<TDto>(List<object> list) where TDto: class            
        {
            string dtoString = typeof(TDto).ToString();
            int dtoIndex = dtoString.IndexOf("Dto");
            string keyPrefix = dtoString.Remove(dtoIndex);
            var serializedList = _serializer.SerializeAllData(list);         
            for (int i = 0; i < serializedList.Count; i++)
            {
                var value = serializedList[i];
                var redisKey = new RedisKey(keyPrefix + "_" + i + 1);
                if (_redisDB.KeyExists(redisKey))
                    throw new Exception("cache key already exists");
                await _redisDB.StringSetAsync(redisKey, new RedisValue(value), null, When.NotExists);
            }
        }
        public List<RedisKey> ListAllKeys(string keyPrefix)
        {
            var key = keyPrefix;
            //"Team";
            //typeof(T).Name;
            int i = 0;
            List<RedisKey> keyList = new List<RedisKey>();
            while (true)
            {
                var redisKey = new RedisKey(key + "_" + i + 1);
                if (_redisDB.KeyExists(redisKey))
                {
                    keyList.Add(redisKey);
                }
                else
                    break;
                i++;
            }
            return keyList;
        }
        public async Task<RedisValue[]> GetAllCacheValues(List<RedisKey> redisKeys)
        {
            return await _redisDB.StringGetAsync(redisKeys.ToArray());
        }
    }
}
