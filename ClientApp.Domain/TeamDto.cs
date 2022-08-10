using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Domain
{
    [Serializable]
    public class TeamDto
    {      
        public int RowNo { get; set; }
        public string Name { get;  set; }
        public int YearFounded { get;  set; }
        public string Description { get; set; }
    }
}
