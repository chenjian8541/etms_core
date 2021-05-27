using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysSmsTemplateDAL
    {
        Task<SysSmsTemplate> GetSysSmsTemplateByTypeDb(int tenantId, int type);

        Task<SysSmsTemplate> GetSysSmsTemplate(int id);

        Task<bool> SaveSysSmsTemplate(SysSmsTemplate entity);

        Task<List<SysSmsTemplate>> GetSysSmsTemplates(int tenantId);

        Task<List<SysSmsTemplate>> GetSysSmsTemplatesAll(int tenantId);

        Task<bool> DelSysSmsTemplate(int id);

        Task<Tuple<IEnumerable<SysSmsTemplate>, int>> GetPaging(AgentPagingBase request);
    }
}
