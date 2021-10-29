using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.TeacherSalary
{
    public interface ITeacherSalaryClassDAL : IBaseDAL
    {
        Task<bool> DelTeacherSalaryClassTimes(long classRecordId);

        Task<bool> SaveTeacherSalaryClassTimes(long classRecordId, List<EtTeacherSalaryClassTimes> entitys);

        Task<bool> DelTeacherSalaryClassDay(DateTime ot);

        Task<bool> SaveTeacherSalaryClassDay(DateTime ot, List<EtTeacherSalaryClassDay> entitys);

        Task<IEnumerable<EtTeacherSalaryClassTimes>> GetTeacherSalaryClassTimes(List<long> teacherIds, DateTime startOt, DateTime endOt);

        Task<Tuple<IEnumerable<TeacherSalaryClassDayView>, int>> GetTeacherSalaryClassDayPaging(IPagingRequest request);

        Task<bool> DelTeacherSalaryClassTimes2(long classRecordId);

        Task<bool> SaveTeacherSalaryClassTimes2(long classRecordId, List<EtTeacherSalaryClassTimes2> entitys);

        Task<IEnumerable<EtTeacherSalaryClassTimes2>> GetTeacherSalaryClassTimes2(List<long> teacherIds, DateTime startOt, DateTime endOt);
    }
}
