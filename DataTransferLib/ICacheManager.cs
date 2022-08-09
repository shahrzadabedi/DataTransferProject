using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTransferLib
{
    public interface ICacheManager
    {
         Task<IEnumerable<TDto>> ReadAllData<TDto>() where TDto : class;
         Task CacheAllData<TDto>(List<TDto> list) where TDto:class;
       
    }
}
