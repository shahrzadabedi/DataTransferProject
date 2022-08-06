using AutoMapper;
using DataTransferProject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public abstract class RepositoryWriter : IRepositoryWriter
    {
        
        protected IMapper _mapper;
        protected ICacheManager _cacheManager;
        public RepositoryWriter(IMapper mapper, ICacheManager cacheManager)
        {            
            _mapper = mapper;
            _cacheManager = cacheManager;
        }
        public async Task WriteToRepository<T,TDto>() where TDto : class where T:class
        {
            var dtoList =await _cacheManager.ReadAllData<TDto>();
            var domainList = dtoList.Select(p => _mapper.Map<T>(p)).ToList();
            await BulkInsertAllValues(domainList.ToArray());
        }
        public abstract Task BulkInsertAllValues(object[] input);       
        //public abstract ArrayList DeserializeValues(RedisValue[] redisValues);
    }
}
