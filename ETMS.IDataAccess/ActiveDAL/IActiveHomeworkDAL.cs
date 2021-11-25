using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
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

        Task UpdateHomeworkAnswerAndReadCount(long homeworkId,int newReadCount,int newFinishCount);

        Task<List<EtActiveHomework>> GetNeedCreateContinuousHomework(DateTime nowDate);

        #region 连续作业
        void AddActiveHomeworkStudent(List<EtActiveHomeworkStudent> entitys);

        Task ResetHomeworkStudentAnswerStatus(long homeworkId);

        Task<HomeworkAnswerAndReadCountView> GetAnswerAndReadCount(long homeworkId);

        Task HomeworkStudentSetReadStatus(long homeworkId, long studentId, byte newStatus);

        Task HomeworkStudentSetAnswerStatus(long homeworkId, long studentId, byte newStatus);

        #endregion 
    }
}
