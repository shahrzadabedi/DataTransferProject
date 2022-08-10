using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataTransferLib
{
    public class RepositoryReader: IRepositoryReader
    {     
        protected ICacheManager _cacheManager;
        protected ISourceDataReader _sourceDataReader;
        public RepositoryReader(ICacheManager cacheManager, ISourceDataReader sourceDataReader)
        {           
            _cacheManager = cacheManager;
            _sourceDataReader = sourceDataReader;
        }
        public async Task ReadFromRepository<TDto>() where TDto: class,new()
        {
            List<TDto> dataList = _sourceDataReader.ReadAll<TDto>();
            await _cacheManager.CacheAllData<TDto>(dataList);
        }
    }
}


