using DataTransferProject;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public abstract class RepositoryReader: IRepositoryReader
    {     
        protected ICacheManager _cacheManager;
        public RepositoryReader(ISerializer serializer,ICacheManager cacheManager)
        {           
            _cacheManager = cacheManager;
        }
        public async Task ReadFromRepository<TDto>() where TDto: class
        {
            List<object> excelDataList = ReadAll<TDto>();
            await _cacheManager.CacheAllData<TDto>(excelDataList);
       //await CacheAllData(excelDataList);
        }
        //public abstract Task CacheAllData(List<string> serializedList);
        public abstract List<object> ReadAll<TDto>() where TDto: class;
    }
}


