using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTransferLib
{
    public interface ISourceDataReader
    {
        List<TDTO> ReadAll<TDTO>() where TDTO : class,new();
    }
}
