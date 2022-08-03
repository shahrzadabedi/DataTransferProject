using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ClientApp
{
    public static class JsonObjectExtension
    {
        public static string ObjectToJson(this Object obj)
        {
            if (obj == null)
                return null;
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static object Deserialize(this string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(input);            
            
        }
    }

}
