using System.Threading.Tasks;

namespace DataTransferLib
{
    public interface ITransferManager
    {
        Task Transfer<T,TDto>() where TDto : class,new() where T:class;
    }
    public class TransferManager : ITransferManager
    {
        private IRepositoryWriter _repositoryWriter { set; get; }
        private IRepositoryReader _repositoryReader { set; get; }
        public TransferManager(IRepositoryWriter repositoryWriter, IRepositoryReader repositoryReader)
        {
            this._repositoryReader = repositoryReader;
            this._repositoryWriter = repositoryWriter;
        }
        public async Task Transfer<T,TDto>() where TDto : class,new() where T:class
        {
            // read from some place and save to in-memory REDIS (or other) cache
            await _repositoryReader.ReadFromRepository<TDto>();
           // read from in-memory REDIS (or other) cache and save to some place
            await _repositoryWriter.WriteToRepository<T,TDto>();
        }
    }
}
