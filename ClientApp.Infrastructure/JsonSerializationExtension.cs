using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public static class JsonObjectExtension
    {
        public static string SerializeToJson(this Object obj)
        {
            if (obj == null)
                return null;
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(this string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(input);            
            
        }
    }

}
