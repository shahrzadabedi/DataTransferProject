using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataTransferProject
{
    public interface IRepositoryReader
    {
        Task ReadFromRepository<T>() where T:class;
    }
}
