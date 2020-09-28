using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IJobAnalyzeDAL : IBaseDAL
    {
        Task UpdateClassTimesRuleLoopStatus();

        Task<Tuple<IEnumerable<LoopClassTimesRule>, int>> GetNeedLoopClassTimesRule(int pageSize, int pageCurrent);

        Task<EtClassTimesRule> GetClassTimesRule(long id);

        Task UpdateClassTimesRule(long id, DateTime lastJobProcessTime);

        Task AddClassTimes(EtClassTimes etClassTimes);

        Task<Tuple<IEnumerable<StudentCourseConsume>, int>> GetNeedConsumeStudentCourse(int pageSize, int pageCurrent, DateTime time);

        Task<EtStudentCourseDetail> GetStudentCourseDetail(long id);

        Task<bool> EditStudentCourseDetail(EtStudentCourseDetail entity);

        Task<Tuple<IEnumerable<HasCourseStudent>, int>> GetHasCourseStudent(int pageSize, int pageCurrent);

        Task<List<EtClassTimes>> GetClassTimesUnRollcall(DateTime classOt);

        Task<List<EtClassTimesStudent>> GetClassTimesStudent(long classTimesId);

        Task<EtClassTimes> GetClassTimes(long classTimesId);

        Task<EtClassRecord> GetClassRecord(long id);

        Task<List<EtClassRecordStudent>> GetClassRecordStudent(long classRecordId);
    }
}
