using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IUserWechatDAL : IBaseDAL
    {
        Task<EtUserWechat> GetUserWechat(long userId);

        Task<bool> SaveUserWechat(EtUserWechat userWechat);
    }
}
