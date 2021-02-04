using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentCourseOpLogDAL: IBaseDAL
    {
        Task<bool> AddStudentCourseOpLog(EtStudentCourseOpLog entity);

        Task<List<EtStudentCourseOpLog>> GetStudentCourseOpLogs(long studentId);
    }
}
