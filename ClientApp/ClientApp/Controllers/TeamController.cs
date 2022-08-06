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
        public async Task SaveAll()
        {
            List<Team> teams = new List<Team>();
            //teams.Add(Team.Create("Shahrzad", 1399, "Test"));
            //teams.Add( Team.Create("Shiva", 1401, "Test2"));
           await transferManager.Transfer<Team,TeamDto>();

        }
    }
}
