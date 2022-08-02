using System.Collections.Generic;

namespace DataTransferProject
{
    public interface IRepositoryReader
    {
        void ReadFromRepository<TData>() where TData : class;
    }
}
