using ClientApp.Domain;
using DataTransferLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {

        private ITransferManager transferManager;
        private IRepositoryReader repositoryReader;
        //private IRepositoryWriter repositoryWriter;
        private ICacheManager cacheManager;
        private Infrastructure.IDBManager dbManager;
        private IDomainDataConverter _domainDataConverter;
        public TeamController(
             ITransferManager transferManager,
             IRepositoryReader reader,
           ICacheManager cacheManager, Infrastructure.IDBManager dbManager,
            IDomainDataConverter domainDataConverter)
        {
            this.transferManager = transferManager;
            repositoryReader = reader;
            //this.repositoryWriter = repositoryWriter;
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
            int pageSize = totalCount / parallelDegree;
            int remainder = totalCount - pageSize * parallelDegree;
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            try
            {
                var tasks = new List<Task>();
                for (int i = 0; i < parallelDegree; i++)
                {
                    tasks.Add(SaveChunkAsync(i, pageSize, parallelDegree, remainder));
                }
                await Task.WhenAll(tasks);
                st.Stop();
            }
            catch (Exception ex)
            {
            }
            return Ok(st.ElapsedMilliseconds);

        }
        [HttpPost, Route("SaveDataSimple")]
        public IActionResult SaveDataWithSimple(int parallelDegree,  int totalCount)
        {
            int pageSize = totalCount / parallelDegree;
            int remainder = totalCount - pageSize * parallelDegree;
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            try
            {
                for (int i = 0; i < parallelDegree; i++)
                {
                    SaveChunk(i, pageSize, parallelDegree, remainder);
                    //cacheManager.UpdateCacheDataAsync<TeamDto>(i * pageSize, (i == parallelDegree - 1 ? remainder : 0) + pageSize);
                }
                st.Stop();
            }
            catch (Exception ex)
            {
            }
            return Ok(st.ElapsedMilliseconds);
        }
        private async Task SaveChunkAsync(int i, int pageSize, int parallelDegree, int remainder)
        {
            var data = cacheManager.ReadDataAsync<TeamDto>(i * pageSize, (i == parallelDegree - 1 ? remainder : 0) + pageSize);
            var arr = _domainDataConverter.Convert<Team, TeamDto>(data.Result);
            await dbManager.BulkInsertAsync(arr);
        }
        private void SaveChunk(int i, int pageSize, int parallelDegree, int remainder)
        {
            var data = cacheManager.ReadData<TeamDto>(i * pageSize, (i == parallelDegree - 1 ? remainder : 0) + pageSize);
            var arr = _domainDataConverter.Convert<Team, TeamDto>(data);
            dbManager.BulkInsert(arr);
        }
    }
}
