using DataTransferProject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class SqlRepositoryWriter : IRepositoryWriter
    {
        private readonly TeamContext dbContext;
        protected readonly IDatabase _redisDB;
        protected readonly IConfiguration _configuration;
        public SqlRepositoryWriter(IConfiguration configuration, TeamContext dbContext,
            IConnectionMultiplexer connectionMultiplexer)
        {
            this._configuration = configuration;
            _redisDB = connectionMultiplexer.GetDatabase();
            this.dbContext = dbContext;
        }
        public async Task WriteToRepository<T>() where T : class             
        {
            //var opts = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
            //IDistributedCache cache = new MemoryDistributedCache(opts);
            var key = typeof(T).Name;
            int i = 0;
            ArrayList arr = new ArrayList();
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

            var redisValues =await _redisDB.StringGetAsync(keyList.ToArray());
            foreach (var item in redisValues)
            {
                var teamDto = item.ToString().Deserialize<TeamDto>();
                var team = teamDto.Map();
                arr.Add(team);
            }
            await dbContext.BulkInsertAsync(arr.ToArray());
        }

    }
}
