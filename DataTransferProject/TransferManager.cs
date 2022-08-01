namespace DataTransferProject
{
    public class TransferManager
    {
        public IRepositoryWriter RepositoryWriter { set; get; }
        public IRepositoryReader RepositoryReader { set; get; }

        public void Transfer<TData>() where TData : class
        {
            var data = RepositoryReader.ReadFromRepository<TData>();
            RepositoryWriter.WriteToRepository(data);
        }
    }
}
