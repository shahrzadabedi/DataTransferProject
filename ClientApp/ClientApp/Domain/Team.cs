using ClientApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Team
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public DateTime YearFounded { get; private set; }
        public string Description { get; private set; }
        
        private List<Player> _players = new(); // we can manipulate the List locally
        
        //Players property is a "defensive copy", users can't modify the field
        public IReadOnlyCollection<Player> Players { get { return _players.AsReadOnly(); } } 
         public Manager Manager { get; private set; }
        public static Team Create(string name, DateTime yearFounded, string description)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(description) || yearFounded.Equals(DateTime.MinValue))
                throw new ArgumentNullException();
            return new Team()
            {
                Id = Guid.NewGuid(),
                Name = name,
                YearFounded = yearFounded,
                Description = description,
                
            };
        }
        public void AddPlayers(IEnumerable<Player> players)
        {
            if (players == null)
            {
                throw new ArgumentNullException(nameof(players));
            }

            if (!players.Any())
            {
                throw new ArgumentException("Must specify at least one player.", nameof(players));
            }
         
            _players.AddRange(players);
        }
        public void ChangeManagement(Manager newManager)
        {
            if (Manager is null || Manager.Name != newManager.Name)
            {
                Manager?.RemoveFromTeam(Id);
                newManager.BecameTeamManager(Id);
                Manager = newManager;
            }
        }
    }
}
