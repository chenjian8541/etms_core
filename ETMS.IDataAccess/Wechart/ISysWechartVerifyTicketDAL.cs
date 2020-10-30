using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Wechart
{
    public interface ISysWechartVerifyTicketDAL
    {
        Task<SysWechartVerifyTicket> GetSysWechartVerifyTicket(string componentAppId);

        Task<bool> SaveSysWechartVerifyTicket(string componentAppId, string componentVerifyTicket);
    }
}
