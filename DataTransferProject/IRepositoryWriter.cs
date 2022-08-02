using System;
using System.Collections.Generic;

namespace DataTransferProject
{
    public interface IRepositoryWriter
    {
        void WriteToRepository<TData>() where TData : class;
    }
}
