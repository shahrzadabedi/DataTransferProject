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
        public SqlRepositoryWriter(IConfiguration configuration,TeamContext dbContext,
            IConnectionMultiplexer connectionMultiplexer)
        {
            this._configuration = configuration;           
            _redisDB = connectionMultiplexer.GetDatabase();
            this.dbContext = dbContext;
        }
        public async Task WriteToRepository()
        {
            //var opts = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
            //IDistributedCache cache = new MemoryDistributedCache(opts);
            var redisValues = _redisDB.ListRange(new RedisKey("Teams"), 0);
                //new RedisKey("2"));
            ArrayList arr = new ArrayList();
            foreach (var redisValue in redisValues)
            {
                var team = redisValue.ToString().<Team>();
                arr.Add(team);
            }
            await dbContext.BulkInsertAsync(arr.ToArray());
        }
        
    }
}
