using DataTransferProject;
using ExcelDataReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class ExcelRepositoryReader : IRepositoryReader
    {
        private string _filePath = "Files\\Teams.xlsx";
        
        public void ReadFromRepository<TData>() where TData : class
        {
            IExcelDataReader reader = null;
            FileStream stream = File.Open(_filePath, FileMode.Open, FileAccess.Read);

            //Must check file extension to adjust the reader to the excel file type

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); 
            if (Path.GetExtension(_filePath).Equals(".xls"))
                reader = ExcelReaderFactory.CreateBinaryReader(stream);           
            else if (Path.GetExtension(_filePath).Equals(".xlsx"))
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            if (reader != null)
            {
                //Fill DataSet
                DataSet content = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                // extract column names
                //var columns = content.Tables[0].Columns.Cast<DataColumn>()
                //  .Select(t => t.ColumnName).ToList();

                var d = content.CreateDataReader();
                // TODO:  inject a logic that gives a list and rule for deciding what class to create

                // now assume that the type decided is 'Team'
                // so the List would be created for Team

                var excelDataList = new ArrayList();
                foreach (DataRow dr in content.Tables[0].Rows)
                {
                    var team = Team.Create(dr.Field<string>("Name"), 2022, "Test");
                    //dr.Field<int>("YearFounded"),
                    //dr.Field<string>("Description"));
                   //var team =  Activator.CreateInstance(typeof(TData));
                    excelDataList.Add(team);
                }
                // Save data to redis
                var opts = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
                IDistributedCache cache = new MemoryDistributedCache(opts);
                
                foreach (var item in excelDataList)
                {
                    string key = "Team_" + Guid.NewGuid().ToString();
                    cache.Set(key, item.ObjectToByteArray());
                   // var cachedData = cache.Get(key);
                }

            }

        }

    }

  
}