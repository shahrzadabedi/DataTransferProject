using ClientApp.Domain;
using DataTransferLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {

        private ITransferManager transferManager;
        private IRepositoryReader repositoryReader;
        private ICacheManager cacheManager;
        private IDBManager dbManager;
        private IDomainDataConverter _domainDataConverter;
        private StringBuilder log = new StringBuilder("");
        public TeamController(
             ITransferManager transferManager,
             IRepositoryReader reader,
           ICacheManager cacheManager, IDBManager dbManager,
            IDomainDataConverter domainDataConverter)
        {
            this.transferManager = transferManager;
            repositoryReader = reader;
            this.cacheManager = cacheManager;
            this.dbManager = dbManager;
            this._domainDataConverter = domainDataConverter;

        }
        [HttpPost]
        public async Task<IActionResult> TransferAll()
        {
            try
            {
                await transferManager.Transfer<Team, TeamDto>();

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); ;
            }
        }

        [HttpGet, Route("GetResult")]
        public IActionResult GetResult([FromBody] TeamDto team)
        {
            return new OkObjectResult(new TeamDto() { Name = "Shahrzad", Description = "test" });
        }
        [HttpPost, Route("CacheData")]
        public async Task<IActionResult> CacheData()
        {
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            await repositoryReader.ReadFromRepository<TeamDto>();
            st.Stop();
            return Ok(st.ElapsedMilliseconds);
        }
        [HttpPost, Route("SaveDataParallel")]
        public async Task<IActionResult> SaveDataParallel(int parallelDegree, int totalCount)
        {
            int chunkeSize = totalCount / parallelDegree;
            int remainder = totalCount - chunkeSize * parallelDegree;
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var tasks = new List<Task>();
                    for (int i = 0; i < parallelDegree; i++)
                    {
                        tasks.Add(SaveChunkAsync(i, chunkeSize, parallelDegree, remainder));
                    }
                    await Task.WhenAll(tasks);
                    scope.Complete();
                    st.Stop();
                }
            }
            catch (Exception ex)
            {
                log.Append(ex.Message);
            }
            return Ok(new { st.ElapsedMilliseconds , log = log.ToString()});

        }
        [HttpPost, Route("SaveDataSimple")]
        public IActionResult SaveDataWithSimple(int parallelDegree, int totalCount)
        {
            int chunkeSize = totalCount / parallelDegree;
            int remainder = totalCount - chunkeSize * parallelDegree;
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            
            st.Start();
            try
            {
                for (int i = 0; i < parallelDegree; i++)
                {
                    SaveChunk(i, chunkeSize, parallelDegree, remainder);
                }
                st.Stop();
            }
            catch (Exception ex)
            {
            }
            return Ok(new { st.ElapsedMilliseconds, log = log.ToString() });
        }
        private async Task SaveChunkAsync(int i, int pageSize, int parallelDegree, int remainder)
        {
            log.Append(string.Format(" i: {0} Start ReadDataAsync, T: {1}{2}", i, DateTime.Now, Environment.NewLine));
            var data = await cacheManager.ReadDataAsync<TeamDto>(i * pageSize, (i == parallelDegree - 1 ? remainder : 0) + pageSize);
          
            log.Append(string.Format(" i: {0} End ReadDataAsync, T: {1}{2}", i, DateTime.Now, Environment.NewLine));
            log.Append(string.Format(" i: {0} Start Convert, T: {1}{2}", i, DateTime.Now, Environment.NewLine));
            var arr = await Task.Run(() => { return _domainDataConverter.Convert<Team, TeamDto>(data); });
            log.Append(string.Format(" i: {0} End Convert, T: {1}{2}", i, DateTime.Now, Environment.NewLine));
            log.Append(string.Format(" i: {0} Start BulkInsertAsync, T: {1}{2}", i, DateTime.Now, Environment.NewLine));
            await dbManager.BulkInsertAsync(arr);
           
        }
        private void SaveChunk(int i, int pageSize, int parallelDegree, int remainder)
        {
            log.Append(string.Format(" i: {0} ReadDataAsync, T: {1}{2}", i, DateTime.Now, Environment.NewLine));
            var data = cacheManager.ReadData<TeamDto>(i * pageSize, (i == parallelDegree - 1 ? remainder : 0) + pageSize);
            var arr = _domainDataConverter.Convert<Team, TeamDto>(data);
            log.Append(string.Format(" i: {0} End ReadData, T: {1}{2}", i, DateTime.Now, Environment.NewLine));
            log.Append(string.Format(" i: {0} Start BulkInsert, T: {1}{2}", i, DateTime.Now, Environment.NewLine));
            dbManager.BulkInsert(arr);
            log.Append(string.Format(" i: {0} End BulkInsert, T: {1}{2}", i, DateTime.Now, Environment.NewLine));
        }
    }
}
