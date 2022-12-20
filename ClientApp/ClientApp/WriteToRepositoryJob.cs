using ClientApp.Domain;
using DataTransferLib;
using Quartz;
using System.Threading.Tasks;

namespace ClientApp.API
{
    public class WriteToRepositoryJob : IJob
    {
        private IRepositoryWriter _writer;
        public WriteToRepositoryJob(IRepositoryWriter repositoryWriter)
        {
            _writer = repositoryWriter;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _writer.WriteToRepository<Team,TeamDto>();
        }
    }
}
