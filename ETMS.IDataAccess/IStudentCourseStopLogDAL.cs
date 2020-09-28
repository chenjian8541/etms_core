using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentCourseStopLogDAL : IBaseDAL
    {
        Task<bool> AddStudentCourseStopLog(EtStudentCourseStopLog log);

        Task<List<EtStudentCourseStopLog>> GetStudentCourseStopLog(long studentId);

        Task<bool> StudentCourseRestore(long studentId, long courseId, DateTime restoreTime);
    }
}
