﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataTransferProject
{
    public interface IRepositoryWriter
    {
        Task WriteToRepository<T,TDTO>() where T:class where TDTO:class;
           
    }
}
