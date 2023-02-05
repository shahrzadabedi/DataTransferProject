//using ClientApp.Domain;
//using DataTransferLib;
//using Quartz;
//using System.Threading.Tasks;

//namespace ClientApp.API
//{
//    public class WriteToRepositoryJob : IJob
//    {
//        private IRepositoryWriter _writer;
//        //private int pageSize;
//        //private int pageNo;
//        public WriteToRepositoryJob(IRepositoryWriter repositoryWriter)
//        {
//            _writer = repositoryWriter;
//            //this.pageSize = pageSize;
//            //this.pageNo = pageNo;
//        }
//        public async Task Execute(IJobExecutionContext context)
//        {
//            //string instName = context.JobDetail.Name;
//            //string instGroup = context.JobDetail.Group;

//            JobDataMap dataMap = context.JobDetail.JobDataMap;

//            int pageSize = dataMap.GetInt("pageSize");
//            int pageNo = dataMap.GetInt("pageNo");
//            await _writer.WriteToRepository<Team,TeamDto>(pageSize,pageNo);
//        }
//    }
//}
