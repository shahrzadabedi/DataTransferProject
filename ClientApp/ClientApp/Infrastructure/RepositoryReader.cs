//using DataTransferProject;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Options;
//using StackExchange.Redis;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace ClientApp.Infrastructure
//{
//    public class RepositoryReader: IRepositoryReader
//    {     
//        protected ICacheManager _cacheManager;
//        protected ISourceDataReader _sourceDataReader;
//        public RepositoryReader(ICacheManager cacheManager, ISourceDataReader sourceDataReader)
//        {           
//            _cacheManager = cacheManager;
//            _sourceDataReader = sourceDataReader;
//        }
//        public async Task ReadFromRepository<TDto>() where TDto: class
//        {
//            List<object> dataList = _sourceDataReader.ReadAll();
//            await _cacheManager.CacheAllData<TDto>(dataList);
//        }
//    }
//}


