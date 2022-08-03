using System.Threading.Tasks;

namespace DataTransferProject
{
    public interface ITransferManager
    {
        Task Transfer<TData>() where TData : class;
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
        public async Task Transfer<TData>() where TData : class
        {
            // read from some place and save to in-memory REDIS cache
            await _repositoryReader.ReadFromRepository();
           // read from in-memory REDIS cache and save to some place
            await _repositoryWriter.WriteToRepository();
        }
    }
}
