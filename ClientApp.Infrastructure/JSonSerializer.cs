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
        public IEnumerable<T> DeserializeAllData<T>(string[] list) where T : class
        {           
            foreach (var item in list)
            {
                var teamDto = item.ToString().Deserialize<T>();
                yield return teamDto;
            }            
        }

        public IEnumerable<string> SerializeAllData<TDTO>(List<TDTO> list)
        {         
            foreach (var item in list)
            {
                yield return item.SerializeToJson();
            }
           
        }
    }
}
