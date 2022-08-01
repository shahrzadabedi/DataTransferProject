using DataTransferProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class ExcelRepositoryReader : IRepositoryReader
    {

        public IList<TData> ReadFromRepository<TData>() where TData : class
        {
            throw new NotImplementedException();
        }
    }
}
