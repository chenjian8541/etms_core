using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.TeacherSalary
{
    public interface ITeacherSalaryPayrollDAL : IBaseDAL
    {

        Task<bool> ExistName(string name);

        Task<TeacherSalaryPayrollBucket> GetTeacherSalaryPayrollBucket(long id);

        Task<long> AddTeacherSalaryPayroll(EtTeacherSalaryPayroll entity);

        Task<long> AddTeacherSalaryPayrollUser(EtTeacherSalaryPayrollUser entity);

        void AddTeacherSalaryPayrollDetail(List<EtTeacherSalaryPayrollUserDetail> userDetails);

        Task<long> AddTeacherSalaryPayrollUserPerformance(EtTeacherSalaryPayrollUserPerformance userPerformance);

        void AddTeacherSalaryPayrollUserPerformanceDetail(List<EtTeacherSalaryPayrollUserPerformanceDetail> performanceDetails);

        Task<bool> SetTeacherSalaryPayStatus(long teacherSalaryPayrollId, byte newStatus);

        Task<bool> SetTeacherSalaryPayStatusIsOK(long teacherSalaryPayrollId, DateTime payDate);

        Task<bool> UpdatePayValue(long payrollId, TeacherSalaryUpdatePayValue teacherSalaryPayroll, TeacherSalaryUpdatePayValue teacherSalaryPayrollUser,
            List<TeacherSalaryUpdatePayValue> teacherSalaryPayrollUserDetails, List<TeacherSalaryUpdatePayValue> teacherSalaryPayrollUserPerformances);

        Task<bool> DelTeacherSalaryPay(long teacherSalaryPayrollId);

        Task<Tuple<IEnumerable<EtTeacherSalaryPayroll>, int>> GetSalaryPayrollPaging(RequestPagingBase request);

        Task<Tuple<IEnumerable<EtTeacherSalaryPayrollUserPerformanceDetail>, int>> GetUserPerformanceDetailPaging(RequestPagingBase request);
    }
}
