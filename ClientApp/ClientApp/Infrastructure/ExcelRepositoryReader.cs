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
    public abstract class ExcelRepositoryReader: IRepositoryReader
    {
        protected string _filePath;
        protected readonly IConfiguration _configuration;
        protected readonly IDatabase _redisDB; 
        public ExcelRepositoryReader(IConfiguration configuration
            ,IConnectionMultiplexer connectionMultiplexer)
        {
            this._configuration = configuration;
            _filePath = _configuration.GetSection("TeamsFilePath").Value;
            _redisDB= connectionMultiplexer.GetDatabase();
        }
        public async Task  ReadFromRepository<T>() where T:class
        {
            var excelDataList = ReadAllFromExcel();          
            var key = typeof(T).Name;
            var pivotRedisValue = excelDataList[0].SerializeToJson();
            for (int i=0;i<excelDataList.Count;i++)
            {                 
                var value = excelDataList[i].SerializeToJson();
                await _redisDB.StringSetAsync(new RedisKey(key+"_"+i+1), new RedisValue(value), null, When.NotExists);
                pivotRedisValue = value;
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
                var team = new TeamDto();
                team.RowNo = (int)dr.Field<double>("RowNo");
                team.Name = dr.Field<string>("Name");
                team.Description = "Test";
                team.YearFounded = 2022;                
                //var team =  Activator.CreateInstance(typeof(TData));
                excelDataList.Add(team);
            }
            return excelDataList;
        }
    }
}


