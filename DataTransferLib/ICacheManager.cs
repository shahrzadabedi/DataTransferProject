using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTransferLib
{
    public interface ICacheManager
    {
        Task<IEnumerable<TDto>> ReadAllDataAsync<TDto>() where TDto : class;
        IEnumerable<TDto> ReadAllData<TDto>() where TDto : class;
        Task<int> CountAllDataAsync<TDto>() where TDto : class;
        Task CacheAllDataAsync<TDto>(List<TDto> list) where TDto:class;
        void  CacheAllData<TDto>(List<TDto> list) where TDto : class;
     
        Task UpdateCacheDataAsync<TDto>(int skip, int? take = null);
        Task<IEnumerable<TDto>> ReadDataAsync<TDto>(int skip, int? take = null) where TDto : class;
        IEnumerable<TDto> ReadData<TDto>(int skip, int? take = null) where TDto : class;
    }
}
