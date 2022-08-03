using DataTransferProject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
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
        protected readonly IConnectionMultiplexer connectionMultiplexer;
        protected readonly IDatabase _redisDB;
        public SqlRepositoryWriter(TeamContext dbContext, IConnectionMultiplexer connectionMultiplexer)
        {
            this.dbContext = dbContext;
        }
        public async Task WriteToRepository()
        {
            var opts = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
            IDistributedCache cache = new MemoryDistributedCache(opts);
            var redisValues = _redisDB.ListRange(new RedisKey("2"));
            ArrayList arr = new ArrayList();
            foreach (var redisValue in redisValues)
            {
                var team = redisValue.ToString().Deserialize();
                arr.Add(team);
            }
            await dbContext.BulkInsertAsync(arr.ToArray());
        }
        
    }
}
