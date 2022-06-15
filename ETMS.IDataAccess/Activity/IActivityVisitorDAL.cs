using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Activity
{
    public interface IActivityVisitorDAL : IBaseDAL
    {
        Task AddActivityVisitor(EtActivityVisitor entity);

        Task<EtActivityVisitor> GetActivityVisitor(long activityId, long miniPgmUserId);
    }
}
