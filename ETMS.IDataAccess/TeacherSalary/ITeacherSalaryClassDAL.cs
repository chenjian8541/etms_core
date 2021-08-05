using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
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

        Task<Tuple<IEnumerable<EtTeacherSalaryClassDay>, int>> GetTeacherSalaryClassDayPaging(IPagingRequest request);
    }
}
