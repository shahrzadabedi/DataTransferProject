using DataTransferLib;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public void BulkInsert(IEnumerable<object> input)
        {
            _dbContext.BulkInsert(input, options =>
           {
               options.EnableConcurrencyForBulkOperation = true;
               options.BatchSize = 100;
           });
        }
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public async Task BulkInsertAsync(IEnumerable<object> input)
        {
            await _semaphore.WaitAsync();
            try
            {
                await _dbContext.BulkInsertAsync(input, options =>
                {
                    options.EnableConcurrencyForBulkOperation = true;
                 });
                
            }
            finally
            {
                _semaphore.Release();
            }

        }
        
    }
}
