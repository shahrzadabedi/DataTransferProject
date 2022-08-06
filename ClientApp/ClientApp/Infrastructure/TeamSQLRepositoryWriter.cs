using AutoMapper;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class TeamSQLRepositoryWriter : RepositoryWriter
    {
        protected readonly TeamContext dbContext;
        public TeamSQLRepositoryWriter( TeamContext dbContext
            ,IMapper mapper,ICacheManager cacheManager) : base(mapper,cacheManager)
            {
            this.dbContext = dbContext;
        }

        public override async Task BulkInsertAllValues(object[] input)
        {
            await dbContext.BulkInsertAsync(input);
        }   
        
    }
}
