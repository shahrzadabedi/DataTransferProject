using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTransferLib
{
    public interface ISerializer
    {
        List<string> SerializeAllData<TDTO>(List<TDTO> list);
        List<T> DeserializeAllData<T>(string[] list) where T:class;
    }
}
