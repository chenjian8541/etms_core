﻿using ETMS.Entity.Common;
using ETMS.Entity.Dto.Open2.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IOpen2BLL : IBaseBLL
    {
        Task<ResponseBase> ClassRecordDetailGet(ClassRecordDetailGetOpenRequest request);
    }
}