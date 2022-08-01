using DataTransferProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class SqlRepositoryWriter : IRepositoryWriter
    {
        private readonly TeamContext teamContext;
        public SqlRepositoryWriter(TeamContext teamContext)
        {
            this.teamContext = teamContext;
        }
        public void WriteToRepository<TData>(IList<TData> data) where TData : class
        {
            teamContext.BulkInsert(data);
            //throw new NotImplementedException();
        }
        
    }
}
