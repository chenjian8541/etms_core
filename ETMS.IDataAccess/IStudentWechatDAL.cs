using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentWechatDAL: IBaseDAL
    {
        Task<EtStudentWechat> GetStudentWechat(string opendId);

        Task<EtStudentWechat> GetStudentWechatByPhone(string phone);

        Task AddStudentWechat(EtStudentWechat entity);

        Task DelStudentWechat(string phone,string openId);
    }
}
