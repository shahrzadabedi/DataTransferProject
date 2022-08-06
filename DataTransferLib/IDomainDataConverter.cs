using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTransferLib
{
    public interface IDomainDataConverter
    {
        IEnumerable<T> Convert<T,TDTO>(IEnumerable<TDTO> list) where T:class where TDTO:class;
    }
}
