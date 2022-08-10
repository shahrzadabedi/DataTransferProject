using ClientApp.Domain;
using DataTransferLib;
using ExcelDataReader;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class ExcelSourceDataReader : ISourceDataReader
    {
        protected string _filePath;
        protected readonly IConfiguration _configuration;
        public ExcelSourceDataReader(IConfiguration configuration)        
        {
            this._configuration = configuration;
            _filePath = _configuration.GetSection("TeamsFilePath").Value;
        }
        public List<TDTO> ReadAll<TDTO>() where TDTO:class, new()
        {
         
            IExcelDataReader reader = null;
            List<TDTO> result = null;
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
                                    
                    var d = content.CreateDataReader();
                    // TODO:  inject a logic that gives a list and rule for deciding what class to create

                    // now assume that the type decided is 'Team'
                    // so the List would be created for Team                   
                    result = ConvertToArrayList<TDTO>(content);
                }
            }
            return result;
        }

        private List<TDTO> ConvertToArrayList<TDTO>(DataSet content) where TDTO: class, new ()
        {
            var excelDataList = new List<TDTO>();
            var ttype = typeof(TDTO).Name;
            var columns = content.Tables[0].Columns.Cast<DataColumn>().Select(t => t.ColumnName).ToList();
            if (ttype == "TeamDto")
            {
                foreach (DataRow dr in content.Tables[0].Rows)
                {
                    var dto = new TDTO();
                    foreach (var col in columns)
                    {
                        if (col=="Name")
                            dto.GetType().GetProperty("Name").SetValue(dto,dr.Field<string>("Name"));                       
                        else if (col=="RowNo")
                            dto.GetType().GetProperty("RowNo").SetValue(dto, (int)dr.Field<double>("RowNo"));
                        
                    }
                    dto.GetType().GetProperty("YearFounded").SetValue(dto,2022);
                    dto.GetType().GetProperty("Description").SetValue(dto, "OK");
                    
                    excelDataList.Add(dto);
                }
            }
            else
                throw new Exception("No other data types to convert");
            return excelDataList;
        }
    }
}
