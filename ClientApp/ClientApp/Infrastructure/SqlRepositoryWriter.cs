using DataTransferProject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class SqlRepositoryWriter : IRepositoryWriter
    {
        private readonly TeamContext dbContext;
        public SqlRepositoryWriter(TeamContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void WriteToRepository<TData>() where TData : class
        {
            //teamContext.BulkInsert(data);
        }
        
    }
}
