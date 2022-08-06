using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class SQLDBManager : IDBManager
    {
        protected readonly TeamContext _dbContext;
        public SQLDBManager(TeamContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task BulkInsertAllValues(object[] input)
        {
            await _dbContext.BulkInsertAsync(input);
        }
    }
}
