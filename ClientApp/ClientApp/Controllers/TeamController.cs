using ClientApp.Infrastructure;
using DataTransferProject;
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
        public TeamController(ITransferManager transferManager)
        {
            this.transferManager = transferManager;
        }
        [HttpPost]
        public async Task<IActionResult> TransferAll()
        {
            List<Team> teams = new List<Team>();
            
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

        
    }
}
