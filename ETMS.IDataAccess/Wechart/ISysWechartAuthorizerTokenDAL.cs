using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Wechart
{
    public interface ISysWechartAuthorizerTokenDAL
    {
        Task<SysWechartAuthorizerToken> GetSysWechartAuthorizerToken(string authorizerAppid);

        Task<bool> SaveSysWechartAuthorizerToken(SysWechartAuthorizerToken entity);
    }
}
