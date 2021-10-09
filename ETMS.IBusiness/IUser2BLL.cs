using ETMS.Entity.Common;
using ETMS.Entity.Dto.User.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IUser2BLL: IBaseBLL
    {
        Task<ResponseBase> GetAllMenusH5(RequestBase request);

        Task<ResponseBase> GetEditMenusH5(RequestBase request);

        Task<ResponseBase> SaveHomeMenusH5(SaveHomeMenusH5Request request);
    }
}
