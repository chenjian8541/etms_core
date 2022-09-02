using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentCourseConsumeLogDAL : IBaseDAL
    {
        void AddStudentCourseConsumeLog(List<EtStudentCourseConsumeLog> studentCourseConsumeLogs);

        Task AddStudentCourseConsumeLog(EtStudentCourseConsumeLog studentCourseConsumeLogs);

        Task<Tuple<IEnumerable<EtStudentCourseConsumeLog>, int>> GetPaging(RequestPagingBase request);

        Task UpdateStudentCourseConsumeLogSurplusCourseDesc(List<UpdateStudentLogOfSurplusCourseView> upLogs);

        Task<DateTime?> GetLastConsumeTime(long studentId, long coueseId);

        Task<CourseConsumeLogDeInfoView> GetDirectDeClassTimesDeSumInfo(DateTime ot);
    }
}
