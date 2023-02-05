using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTransferLib
{
    public interface IDBManager
    {
        Task BulkInsertAllValues(object[] input);
        
        Task BulkInsertAsync(IEnumerable<object> input);
        void BulkInsert(IEnumerable<object> input);
    }

}
