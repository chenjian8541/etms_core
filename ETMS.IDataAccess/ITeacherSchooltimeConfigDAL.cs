using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View.Rq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITeacherSchooltimeConfigDAL : IBaseDAL
    {
        Task<TeacherSchooltimeConfigBucket> TeacherSchooltimeConfigGet(long teacherId);

        Task AddTeacherSchooltimeConfig(EtTeacherSchooltimeConfig entity, List<EtTeacherSchooltimeConfigDetail> details);

        Task DelTeacherSchooltimeConfig(long schooltimeConfigId,long teacherId);

        Task SaveTeacherSchooltimeConfigExclude(EtTeacherSchooltimeConfigExclude excludeConfig);

        Task ResetTeacherSchooltimeConfig(ResetTeacherSchooltimeConfigInput input);
    }
}
