using ClientApp.Domain;
using ClientApp.Infrastructure;
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
        private IRepositoryWriter repositoryWriter;
        public TeamController(ITransferManager transferManager,IRepositoryReader reader, 
            IRepositoryWriter repositoryWriter)
        {
            this.transferManager = transferManager;
            repositoryReader = reader;
            this.repositoryWriter = repositoryWriter;
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
        public  IActionResult GetResult([FromBody]  TeamDto team)
        {
            return new OkObjectResult(new TeamDto() { Name = "Shahrzad", Description = "test" });
        }
        [HttpPost,Route("CacheData")]
        public async Task<IActionResult> CacheData()
        {
            await repositoryReader.ReadFromRepository<TeamDto>();
            return Ok();
        }
        [HttpPost, Route("SaveData")]
        public async Task<IActionResult> SaveData()
        {
            await repositoryWriter.WriteToRepository<Team,TeamDto>();
            return Ok();
        }
        
    }
}
