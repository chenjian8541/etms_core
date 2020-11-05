﻿using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IActiveWxMessageDAL
    {
        Task<bool> AddActiveWxMessage(EtActiveWxMessage entity);

        Task<EtActiveWxMessage> GetActiveWxMessage(long id);

        Task<bool> EditActiveWxMessage(EtActiveWxMessage entity);

        Task<bool> DelActiveWxMessage(long id);

        Task<Tuple<IEnumerable<EtActiveWxMessage>, int>> GetPaging(IPagingRequest request);

        bool AddActiveWxMessageDetail(List<EtActiveWxMessageDetail> entitys);

        Task<bool> EditActiveWxMessageDetail(EtActiveWxMessageDetail entity);

        Task<EtActiveWxMessageDetail> GetActiveWxMessageDetail(long id);

        Task<Tuple<IEnumerable<EtActiveWxMessageDetail>, int>> GetDetailPaging(IPagingRequest request);
    }
}
