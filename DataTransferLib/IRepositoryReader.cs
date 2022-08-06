using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataTransferLib
{
    public interface IRepositoryReader
    {
        Task ReadFromRepository<TDto>() where TDto: class;
    }
}
