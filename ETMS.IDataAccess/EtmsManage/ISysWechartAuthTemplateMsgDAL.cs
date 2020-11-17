using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysWechartAuthTemplateMsgDAL
    {
        Task<List<SysWechartAuthTemplateMsg>> GetSysWechartAuthTemplateMsg(string authorizerAppid);

        Task<SysWechartAuthTemplateMsg> GetSysWechartAuthTemplateMsg(string authorizerAppid, string templateIdShort);

        Task<bool> SaveSysWechartAuthTemplateMsg(string authorizerAppid, string templateIdShort, string templateId);
    }
}
