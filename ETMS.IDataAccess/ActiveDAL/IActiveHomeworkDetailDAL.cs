using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IActiveHomeworkDetailDAL : IBaseDAL
    {
        bool AddActiveHomeworkDetail(List<EtActiveHomeworkDetail> homeworkDetails);

        Task<bool> EditActiveHomeworkDetail(EtActiveHomeworkDetail homeworkDetail);

        Task<ActiveHomeworkDetailBucket> GetActiveHomeworkDetailBucket(long id);

        Task<List<EtActiveHomeworkDetail>> GetActiveHomeworkDetail(long homeworkId, byte answerStatus);

        Task<List<EtActiveHomeworkDetail>> GetActiveHomeworkDetail(long homeworkId);

        Task<Tuple<IEnumerable<EtActiveHomeworkDetail>, int>> GetPaging(IPagingRequest request);

        Task<bool> AddActiveHomeworkDetailComment(EtActiveHomeworkDetailComment detailComment);

        Task<bool> DelActiveHomeworkDetailComment(long detailId, long id);
    }
}
