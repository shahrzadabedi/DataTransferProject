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
    public class RepositoryWriter : IRepositoryWriter
    {
        
        protected IMapper _mapper;
        protected ICacheManager _cacheManager;
        protected IDBManager _dbManager;
        public RepositoryWriter(IMapper mapper, ICacheManager cacheManager, IDBManager dbManager)
        {            
            _mapper = mapper;
            _cacheManager = cacheManager;
            _dbManager = dbManager;
        }
        public async Task WriteToRepository<T,TDto>() where TDto : class where T:class
        {
            var dtoList =await _cacheManager.ReadAllData<TDto>();
            var domainList = dtoList.Select(p => _mapper.Map<T>(p)).ToList();
            await _dbManager.BulkInsertAllValues(domainList.ToArray());
        }      
    }
}
