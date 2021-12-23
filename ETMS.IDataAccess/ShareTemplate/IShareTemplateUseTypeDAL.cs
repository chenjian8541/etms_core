using ETMS.Entity.CacheBucket.ShareTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.ShareTemplate
{
    public interface IShareTemplateUseTypeDAL : IBaseDAL
    {
        Task<ShareTemplateUseTypeBucket> GetShareTemplate(int useType);

        Task UpdateShareTemplate(int useType);
    }
}
