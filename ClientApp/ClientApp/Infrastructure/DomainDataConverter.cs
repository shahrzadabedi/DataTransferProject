using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class DomainDataConverter : IDomainDataConverter
    {
        protected IMapper _mapper;
        public DomainDataConverter(IMapper mapper) 
        {
            _mapper = mapper;
        }
        public IEnumerable<T> Convert<T, TDTO>(IEnumerable<TDTO> dtoList)
            where T : class
            where TDTO : class
        {
            return dtoList.Select(p => _mapper.Map<T>(p));
        }
    }
}
