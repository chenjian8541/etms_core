using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    internal static class ComBusiness6
    {
        public static async Task<IEnumerable<BaseClassStudent>> GetClassStudent(ClassBucket myClassBucket,
            IClassTimesRuleStudentDAL classTimesRuleStudentDAL, long ruleId)
        {
            if (myClassBucket == null || myClassBucket.EtClass == null || myClassBucket.EtClassStudents == null ||
                myClassBucket.EtClassStudents.Count == 0)
            {
                return new List<EtClassStudent>();
            }
            if (ruleId == 0)
            {
                return myClassBucket.EtClassStudents;
            }
            var classTimesRuleStudents = await classTimesRuleStudentDAL.GetClassTimesRuleStudent(myClassBucket.EtClass.Id, ruleId);
            if (classTimesRuleStudents != null && classTimesRuleStudents.Any())
            {
                return classTimesRuleStudents;
            }
            return myClassBucket.EtClassStudents;
        }
    }
}
