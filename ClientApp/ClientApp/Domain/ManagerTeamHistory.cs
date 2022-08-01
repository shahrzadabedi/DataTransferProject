using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Domain
{
    public class ManagerTeamHistory
    {
        public ManagerTeamHistory(Guid managerId, Guid teamId)
        {
            ManagerId = managerId;
            TeamId = teamId;
        }

        private ManagerTeamHistory()
        {
        }

        public Guid ManagerId { get; private set; }
        public Guid TeamId { get; private set; }
    }
}
