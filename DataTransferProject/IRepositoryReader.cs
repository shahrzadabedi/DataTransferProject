using System.Collections.Generic;

namespace DataTransferProject
{
    public interface IRepositoryReader
    {
        IList<TData> ReadFromRepository<TData>() where TData : class;
    }
}
