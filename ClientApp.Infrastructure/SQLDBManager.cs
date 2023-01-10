using DataTransferLib;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    //public interface IDBManager
    //{
    //    Task BulkInsertAllValues(object[] input);        
    //    Task BulkInsertAsync(IEnumerable<object> input);
    //    void BulkInsert(IEnumerable<object> input);
    //}
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
        public async Task BulkInsertAsync(IEnumerable<object> input) 
        {             
             await _dbContext.BulkInsertAsync(input, options => 
             {
                 options.UseParallel = true; 
                 options.EnableConcurrencyForBulkOperation = true;               
                 options.BatchSize = 100;
                 options.DisableValueGenerated = true;
             });
           
        }
    }
}
