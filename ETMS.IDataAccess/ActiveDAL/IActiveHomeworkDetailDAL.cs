using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
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

        Task<List<EtActiveHomeworkDetail>> GetActiveHomeworkDetail(long homeworkId, byte answerStatus, DateTime? otDate, long? studentId);

        Task<List<EtActiveHomeworkDetail>> GetActiveHomeworkDetail(long homeworkId, DateTime otDate);

        Task<Tuple<IEnumerable<EtActiveHomeworkDetail>, int>> GetPaging(IPagingRequest request);

        Task<bool> AddActiveHomeworkDetailComment(EtActiveHomeworkDetailComment detailComment);

        Task<bool> DelActiveHomeworkDetailComment(long detailId, long id);

        Task<IEnumerable<EtActiveHomeworkDetail>> GetHomeworkDetailTomorrowSingleWorkExDate();

        Task<IEnumerable<EtActiveHomeworkDetail>> GetHomeworkDetailContinuousWorkTodayNotAnswer(DateTime otDate, int maxTime, int minTime);

        Task<byte> GetHomeworkStudentAnswerStatus(long homeworkId, long studentId);

        Task<bool> ExistHomeworkDetail(long homeworkId, DateTime otDate);

        Task UpdateHomeworkDetail(long homeworkId,string title,string workContent,string workMedias);
    }
}
