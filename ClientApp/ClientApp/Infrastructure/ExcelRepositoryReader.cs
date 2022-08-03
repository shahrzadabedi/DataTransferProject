using DataTransferProject;
using ExcelDataReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public interface IRedisCacheService
    {
        T Get<T>(string key);
        T Set<T>(string key, T value);
    }
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            var value = _cache.GetString(key);

            if (value != null)
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }
        public T Set<T>(string key, T value)
        {
            var timeOut = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                SlidingExpiration = TimeSpan.FromMinutes(60)
            };

            _cache.SetString(key, JsonSerializer.Serialize(value), timeOut);

            return value;
        }

    }
    public abstract class ExcelRepositoryReader: IRepositoryReader
    {
        protected string _filePath;
        protected readonly IConfiguration _configuration;
        protected readonly IRedisCacheService _redisCacheService;
        protected readonly IConnectionMultiplexer connectionMultiplexer;
        protected readonly IDatabase _redisDB; 
        public ExcelRepositoryReader(IConfiguration configuration
            ,IConnectionMultiplexer connectionMultiplexer)
        {
            this._configuration = configuration;
            _filePath = _configuration.GetSection("TeamsFilePath").Value;
            _redisDB= connectionMultiplexer.GetDatabase();
        }
        public async Task  ReadFromRepository()
        {
            var excelDataList = ReadAllFromExcel();
            //var opts = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
           // IDistributedCache cache = new MemoryDistributedCache(opts);
            foreach (var item in excelDataList)
            {
                string key = "Team_" + Guid.NewGuid().ToString();
                await _redisDB.StringSetAsync(key, item.ObjectToJson());
            }
            
        }
        public abstract ArrayList ReadAllFromExcel();
    }
    public class TeamExcelRepositoryReader : ExcelRepositoryReader
    {
        public TeamExcelRepositoryReader(IConfiguration configuration, IConnectionMultiplexer connectionMultiplexer) : base(configuration,connectionMultiplexer) { }
        public override ArrayList ReadAllFromExcel()
        {
            IExcelDataReader reader = null;
            var excelDataList = new ArrayList();
            using (FileStream stream = File.Open(_filePath, FileMode.Open, FileAccess.Read))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                if (Path.GetExtension(_filePath).Equals(".xls"))
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                else if (Path.GetExtension(_filePath).Equals(".xlsx"))
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                if (reader != null)
                {
                    DataSet content = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    //var columns = content.Tables[0].Columns.Cast<DataColumn>().Select(t => t.ColumnName).ToList();                    
                    var d = content.CreateDataReader();
                    // TODO:  inject a logic that gives a list and rule for deciding what class to create

                    // now assume that the type decided is 'Team'
                    // so the List would be created for Team                   
                    excelDataList =  ConvertToArrayList(content);                    
                }
            }
            return excelDataList;
        }
        private ArrayList ConvertToArrayList(DataSet content)
        {
            var excelDataList = new ArrayList();
            foreach (DataRow dr in content.Tables[0].Rows)
            {
                var team = Team.Create(dr.Field<string>("Name"), 2022, "Test");
                //dr.Field<int>("YearFounded"),
                //dr.Field<string>("Description"));
                //var team =  Activator.CreateInstance(typeof(TData));
                excelDataList.Add(team);
            }
            return excelDataList;
        }
    }
}


