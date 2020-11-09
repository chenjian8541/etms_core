using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IActiveWxMessageParentReadDAL: IBaseDAL
    {
        Task UpdateParentRead(string phone,List<long> studentIds);

        Task<int> GetParentUnreadCount(string phone, List<long> studentIds);
    }
}
