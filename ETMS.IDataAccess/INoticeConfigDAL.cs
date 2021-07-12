using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface INoticeConfigDAL : IBaseDAL
    {
        Task SaveNoticeConfig(EtNoticeConfig entity);

        Task<List<EtNoticeConfig>> GetNoticeConfig(int type);

        Task<EtNoticeConfig> GetNoticeConfig(int type, byte peopleType, int scenesType);
    }
}
