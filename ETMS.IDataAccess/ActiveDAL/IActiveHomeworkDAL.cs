using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IActiveHomeworkDAL : IBaseDAL
    {
        Task<bool> AddActiveHomework(EtActiveHomework entity);

        Task<bool> EditActiveHomework(EtActiveHomework entity);

        Task<EtActiveHomework> GetActiveHomework(long id);

        Task<bool> DelActiveHomework(long id);

        Task<Tuple<IEnumerable<EtActiveHomework>, int>> GetPaging(IPagingRequest request);
    }
}
