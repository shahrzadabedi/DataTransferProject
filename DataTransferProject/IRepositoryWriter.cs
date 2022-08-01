using System;
using System.Collections.Generic;

namespace DataTransferProject
{
    public interface IRepositoryWriter
    {
        void WriteToRepository<TData>(IList<TData> data) where TData : class;
    }
}
