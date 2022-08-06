﻿using DataTransferLib;
using ExcelDataReader;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
        public List<object> ReadAll()
        {
         
            IExcelDataReader reader = null;
            List<object> result = null;
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
                    result = ConvertToArrayList(content);
                }
            }
            return result;
        }

        private List<object> ConvertToArrayList(DataSet content)
        {
            var excelDataList = new List<object>();
            foreach (DataRow dr in content.Tables[0].Rows)
            {
                var team = new TeamDto();
                team.RowNo = (int)dr.Field<double>("RowNo");
                team.Name = dr.Field<string>("Name");
                team.Description = "Test";
                team.YearFounded = 2022;
                excelDataList.Add(team);
            }
            return excelDataList;
        }
    }
}
