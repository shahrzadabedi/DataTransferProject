using DataTransferLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class JSonSerializer : ISerializer
    {
        public List<T> DeserializeAllData<T>(string[] list) where T : class
        {
            List<T> result = new List<T>();
            foreach (var item in list)
            {
                var teamDto = item.ToString().Deserialize<T>();
                result.Add(teamDto);
            }
            return result;
        }

        public List<string> SerializeAllData<TDTO>(List<TDTO> list)
        {
            List<string> result = new List<string>();
            foreach (var item in list)
            {
                result.Add(item.SerializeToJson());
            }
            return result;
        }
    }
}
