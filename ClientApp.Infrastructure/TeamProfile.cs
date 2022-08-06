using AutoMapper;
using ClientApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<TeamDto,Team>().ConstructUsing(x => Team.Create(x.RowNo,x.Name,x.YearFounded,x.Description));
        }
    }
}
