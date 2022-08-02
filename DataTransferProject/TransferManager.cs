namespace DataTransferProject
{
    public interface ITransferManager
    {
        void Transfer<TData>() where TData : class;
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
        public void Transfer<TData>() where TData : class
        {
            // read from some place and save to in-memory REDIS cache
            _repositoryReader.ReadFromRepository<TData>();
           // read from in-memory REDIS cache and save to some place
            _repositoryWriter.WriteToRepository<TData>();
        }
    }
}
