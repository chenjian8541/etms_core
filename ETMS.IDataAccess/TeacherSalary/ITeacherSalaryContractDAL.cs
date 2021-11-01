using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.TeacherSalary
{
    public interface ITeacherSalaryContractDAL : IBaseDAL
    {
        Task<TeacherSalaryContractBucket> GetTeacherSalaryContract(long teacherId);

        Task<bool> SaveTeacherSalaryContract(long teacherId, List<EtTeacherSalaryContractFixed> fixeds, EtTeacherSalaryContractPerformanceSet performanceSet,
            List<EtTeacherSalaryContractPerformanceSetDetail> performanceSetDetails);

        Task ClearTeacherSalaryContractPerformance(long teacherId);
    }
}
