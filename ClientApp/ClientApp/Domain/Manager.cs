using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Domain
{
    [Serializable]
    public class Manager
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Family { get; private set; }
        public Guid? CurrentTeamId { get; private set; }
        public List<ManagerTeamHistory> PastTeams { get; private set; }

        public void RemoveFromTeam(Guid oldTeamId)
        {
            CurrentTeamId = null; // Guid.Empty;
            PastTeams.Add(new ManagerTeamHistory(Id, oldTeamId));
        }

        public void BecameTeamManager(Guid newTeamId)
        {
            if (CurrentTeamId != Guid.Empty && CurrentTeamId.HasValue)
            {
                PastTeams.Add(new ManagerTeamHistory(Id, CurrentTeamId.Value));
            }
            CurrentTeamId = newTeamId;
        }
    }
}
