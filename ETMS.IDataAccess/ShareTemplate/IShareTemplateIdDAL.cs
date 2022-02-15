using ETMS.Entity.CacheBucket.ShareTemplate;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.ShareTemplate
{
    public interface IShareTemplateIdDAL : IBaseDAL
    {
        Task<bool> IsInitializeSystemData();

        Task<EtShareTemplate> GetShareTemplate(long id);

        Task AddShareTemplate(EtShareTemplate entity);

        void AddShareTemplate(List<EtShareTemplate> entitys);

        Task EditShareTemplate(EtShareTemplate entity);

        Task DelShareTemplate(long id, int useType);

        Task ChangeShareTemplateStatus(EtShareTemplate entity, byte newStatus);

        Task<Tuple<IEnumerable<EtShareTemplate>, int>> GetPaging(RequestPagingBase request);
    }
}
