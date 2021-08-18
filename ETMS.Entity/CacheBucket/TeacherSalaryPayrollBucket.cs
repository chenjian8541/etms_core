using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class TeacherSalaryPayrollBucket : ICacheDataContract
    {
        public EtTeacherSalaryPayroll TeacherSalaryPayroll { get; set; }

        public List<EtTeacherSalaryPayrollUser> TeacherSalaryPayrollUsers { get; set; }

        public List<EtTeacherSalaryPayrollUserDetail> TeacherSalaryPayrollUserDetails { get; set; }

        public List<EtTeacherSalaryPayrollUserPerformance> TeacherSalaryPayrollUserPerformances { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构_ID
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"TeacherSalaryPayrollBucket_{parms[0]}_{parms[1]}";
        }
    }
}
