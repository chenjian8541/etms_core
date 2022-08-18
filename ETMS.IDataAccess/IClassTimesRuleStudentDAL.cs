using ETMS.Entity.Database.Source;
using ETMS.Entity.View.OnlyOneFiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IClassTimesRuleStudentDAL : IBaseDAL
    {
        Task<bool> ExistStudent(long studentId, long classId, long ruleId);

        Task<List<EtClassTimesRuleStudent>> GetClassTimesRuleStudent(long classId, long ruleId);

        Task AddClassTimesRuleStudent(List<EtClassTimesRuleStudent> entitys);

        Task DelClassTimesRuleStudent(long id, long classId, long ruleId);

        Task DelClassTimesRuleStudentByStudentId(long studentId, long classId);

        Task ClearClassTimesRuleStudent(long classId, List<long> ruleIds);

        Task<IEnumerable<OnlyOneFiledRuleId>> GetIsSetRuleStudent(long classId);
    }
}
