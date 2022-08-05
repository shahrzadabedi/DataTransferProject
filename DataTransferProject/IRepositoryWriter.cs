using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataTransferProject
{
    public interface IRepositoryWriter
    {
        Task WriteToRepository<T>()
            where T : class;
    }
}
