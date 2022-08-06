using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Domain
{
    [Serializable]
    public class Player
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Family { get; private set; }
    }
}
