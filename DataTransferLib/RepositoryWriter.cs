using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTransferLib
{
    public class RepositoryWriter : IRepositoryWriter
    {
        protected ICacheManager _cacheManager;
        protected IDBManager _dbManager;
        protected IDomainDataConverter _domainDataConverter;
        public RepositoryWriter( ICacheManager cacheManager, IDBManager dbManager, IDomainDataConverter domainDataConverter)
        {     
            _cacheManager = cacheManager;
            _domainDataConverter = domainDataConverter;
            _dbManager = dbManager;
        }
        public async Task WriteToRepository<T,TDto>() where TDto : class where T:class
        {
            var dtoList =await _cacheManager.ReadAllData<TDto>();
            var domainList = _domainDataConverter.Convert<T,TDto>(dtoList).ToList();
            await _dbManager.BulkInsertAllValues(domainList.ToArray());
        }      
    }
}
