using DataTransferLib;
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
        public async Task<IEnumerable<TDto>> ReadAllDataAsync<TDto>() where TDto : class
        {
            RedisValue[] cacheValues = await DetermineValuesAsync<TDto>(0);
            var list = cacheValues.ToList().Select(p => p.ToString()).ToArray();
            var dtoList = _serializer.DeserializeAllData<TDto>(list);
            return dtoList;
        }
        public async Task<int> CountAllDataAsync<TDto>() where TDto : class
        {
            RedisValue[] cacheValues = await DetermineValuesAsync<TDto>(0);
            return cacheValues.Length;
        }
        public async Task<IEnumerable<TDto>> ReadDataAsync<TDto>(int skip,int? take= null) where TDto: class
        {
            RedisValue[] cacheValues = await DetermineValuesAsync<TDto>(skip, take);
            var list = cacheValues.ToList().Select(p => p.ToString()).ToArray();
            var dtoList = _serializer.DeserializeAllData<TDto>(list);
            return dtoList;
        }
        private async Task<RedisValue[]> DetermineValuesAsync<TDto>(int skip, int? take= null) where TDto : class
        {
            string dtoString = typeof(TDto).Name;
            int dtoIndex = dtoString.IndexOf("Dto");
            string keyPrefix = dtoString.Remove(dtoIndex); ;
            var keyList = ListAllKeys(keyPrefix,skip, take);
            RedisValue[] cacheValues = await GetAllCacheValuesAsync(keyList);
            return cacheValues;
        }
        private RedisValue[] DetermineValues<TDto>(int skip, int? take = null) where TDto : class
        {
            string dtoString = typeof(TDto).Name;
            int dtoIndex = dtoString.IndexOf("Dto");
            string keyPrefix = dtoString.Remove(dtoIndex); ;
            var keyList = ListAllKeys(keyPrefix, skip, take);
            RedisValue[] cacheValues =  GetAllCacheValues(keyList);
            return cacheValues;
        }

        public async Task CacheAllDataAsync<TDto>(List<TDto> list) where TDto: class            
        {
            string dtoString = typeof(TDto).Name;
            int dtoIndex = dtoString.IndexOf("Dto");
            string keyPrefix = dtoString.Remove(dtoIndex);
            var serializedList = _serializer.SerializeAllData(list).ToList();         
            for (int i = 0; i < serializedList.Count; i++)
            {
                var value = serializedList[i];
                var redisKey = new RedisKey(keyPrefix + ":" + i + 1+":"+false.ToString());
                if (_redisDB.KeyExists(redisKey))
                    throw new Exception("cache key already exists");
                await _redisDB.StringSetAsync(redisKey, new RedisValue(value), null, When.NotExists);
            }
        }
        public async Task UpdateCacheDataAsync<TDto>(int skip,int? take = null) 
        {
            string dtoString = typeof(TDto).Name;
            int dtoIndex = dtoString.IndexOf("Dto");
            string keyPrefix = dtoString.Remove(dtoIndex); 
            var keyList = ListAllKeys(keyPrefix,skip,take);
            var falseLength = "false".Length + 1;
            keyList.Select(p => p.ToString().Substring(0, p.ToString().Length - falseLength));
            foreach (var item in keyList)
            {
                var newKey = item.ToString().Substring(0, item.ToString().Length - falseLength);
                newKey += ":true";
                await _redisDB.KeyRenameAsync(item, new RedisKey(newKey));
            }

        }        
        public async Task<RedisValue[]> GetAllCacheValuesAsync(IEnumerable<RedisKey> redisKeys)
        {
            return await _redisDB.StringGetAsync(redisKeys.ToArray());
        }
      
        public  RedisValue[] GetAllCacheValues(IEnumerable<RedisKey> redisKeys)
        {
            return _redisDB.StringGet(redisKeys.ToArray());
        }
        private IEnumerable<RedisKey> ListAllKeys(string keyPrefix,int skip, int? take = null)
        {
            var key = keyPrefix;
            int i = skip;
           
            //List<RedisKey> keyList = new List<RedisKey>();
            int count = 0;
            while ((count< take && take.HasValue ) || !take.HasValue  )
            {
                var redisKeyFalse = new RedisKey(key + ":" + i + 1 + ":" + false.ToString());
                var redisKeyTrue  = new RedisKey(key + ":" + i + 1 + ":" + true.ToString());
                if (!_redisDB.KeyExists(redisKeyFalse) && !_redisDB.KeyExists(redisKeyTrue))
                {
                    break;
                    //keyList.Add(redisKey);
                }
                else if (_redisDB.KeyExists(redisKeyFalse))
                {
                    yield return redisKeyFalse;
                    i++;
                    count++;
                }
                else
                {
                    i++;
                    continue;                    
                }
            }
            //int totalPages = (int)Math.Ceiling((decimal)count / pageSize);
            //if (pageNo > totalPages)
            // throw new Exception("Exception");            
            //return keyList;
                //.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<TDto> ReadAllData<TDto>() where TDto : class
        {
            string dtoString = typeof(TDto).Name;
            int dtoIndex = dtoString.IndexOf("Dto");
            string keyPrefix = dtoString.Remove(dtoIndex); ;
            var keyList = ListAllKeys(keyPrefix,0);
            RedisValue[] cacheValues =  GetAllCacheValues(keyList);
            var list = cacheValues.ToList().Select(p => p.ToString()).ToArray();
            var dtoList = _serializer.DeserializeAllData<TDto>(list);
            return dtoList;
        }

        public void CacheAllData<TDto>(List<TDto> list) where TDto : class
        {
            string dtoString = typeof(TDto).Name;
            int dtoIndex = dtoString.IndexOf("Dto");
            string keyPrefix = dtoString.Remove(dtoIndex);
            var serializedList = _serializer.SerializeAllData(list).ToList();
            for (int i = 0; i < serializedList.Count; i++)
            {
                var value = serializedList[i];
                var redisKey = new RedisKey(keyPrefix + ":" + i + 1 + ":" + false.ToString());
                if (_redisDB.KeyExists(redisKey))
                    throw new Exception("cache key already exists");
                 _redisDB.StringSet(redisKey, new RedisValue(value), null, When.NotExists);
            }
        }

        public IEnumerable<TDto> ReadData<TDto>(int skip, int? take = null) where TDto : class
        {
            RedisValue[] cacheValues =  DetermineValues<TDto>(skip, take);
            var list = cacheValues.ToList().Select(p => p.ToString()).ToArray();
            var dtoList = _serializer.DeserializeAllData<TDto>(list);
            return dtoList;
        }
    }
}
