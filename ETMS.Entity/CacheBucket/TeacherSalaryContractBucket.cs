using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket
{
    public class TeacherSalaryContractBucket : ICacheDataContract
    {
        public List<EtTeacherSalaryContractFixed> TeacherSalaryContractFixeds { get; set; }

        public EtTeacherSalaryContractPerformanceSet TeacherSalaryContractPerformanceSet { get; set; }

        public List<EtTeacherSalaryContractPerformanceSetDetail> TeacherSalaryContractPerformanceSetDetails { get; set; }

        public List<EtTeacherSalaryContractPerformanceLessonBasc> EtTeacherSalaryContractPerformanceLessonBascs { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"TeacherSalaryContractBucket_{parms[0]}_{parms[1]}";
        }
    }
}
